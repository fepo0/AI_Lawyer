import requests
from bs4 import BeautifulSoup
import mysql.connector

conn = mysql.connector.connect(
    host='localhost',
    user='root',
    password='12345',
    database='LawsDB'
)
cursor = conn.cursor()

url = "https://kanstytucyja.online/index.php/text/ru"

response = requests.get(url)
soup = BeautifulSoup(response.content, 'html.parser')

articles = soup.find_all("h3")

for article in articles:
    title = article.get_text(strip=True)
    content = ""

    next_node = article.find_next_sibling()
    while next_node and next_node.name == "p":
        content += next_node.get_text(strip=True) + "\n"
        next_node = next_node.find_next_sibling()

    if "статья" in title:
        article_number = title.replace("Статья", "").strip()
    else:
        article_number = None

    if article_number:
        cursor.execute("""
        INSERT INTO Laws (ArticleNumber, Title, Content)
        VALUES (%s, %s, %s)
        ON DUPLICATE KEY UPDATE Title=VALUES(Title), Content=VALUES(Content)
        """, (article_number, title, content))

conn.commit()
cursor.close()
conn.close()
print("Парсинг закончен.")