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

url = "https://pravo.by/document/?guid=3871&p0=H09900275"

response = requests.get(url)
soup = BeautifulSoup(response.content, 'html.parser')

articles = soup.find_all("a", href=True)

for article in articles:
    title_text = article.get_text(strip=True)
    href = article['href']

    if "статья" in title_text.lower() and href.startswith("/document"):
        article_number = title_text.split(" ")[1]
        full_url = "https://pravo.by" + href

        article_page = requests.get(full_url)
        article_soup = BeautifulSoup(article_page.content, 'html.parser')

        content_div = article_soup.find_all("div", class_="content")
        if content_div:
            content = content_div.get_text(strip=True)

            try:
                cursor.execute("""
                INSERT INTO Laws (ArticleNumber, Title, Content)
                VALUES (%s, %s, %s)
                ON DUPLICATE KEY UPDATE Title=VALUES(Title), Content=VALUES(Content)
                """, (article_number, title_text, content))
                print(f"Добавленно {article_number} - {title_text}")
            except Exception as e:
                print(f"Ошиюбка при добавлении статьи {article_number}: {e}")

conn.commit()
cursor.close()
conn.close()
print("Парсинг закончен.")