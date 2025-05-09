import requests
from bs4 import BeautifulSoup
import mysql.connector
import re
import time

# Подключение к базе данных
conn = mysql.connector.connect(
    host='localhost',
    user='root',
    password='12345',
    database='LawsDB'
)
cursor = conn.cursor()

# URL страницы Конституции
url = "https://president.gov.by/ru/gosudarstvo/constitution"

headers = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
}

try:
    response = requests.get(url, headers=headers, timeout=10)
    response.raise_for_status()
except requests.exceptions.RequestException as e:
    print(f"Ошибка подключения: {e}")
    exit()

soup = BeautifulSoup(response.content, 'html.parser')

# Найдем все элементы, содержащие статьи
# Предположим, что каждая статья начинается с заголовка h3
articles = soup.find_all(['h3', 'p'])

article_number = ''
title = ''
content = ''
collecting = False

for tag in articles:
    if tag.name == 'h3':
        text = tag.get_text(strip=True)
        match = re.match(r'Статья\s+(\d+[а-яА-ЯёЁ\d]*)\.', text)
        if match:
            # Сохраняем предыдущую статью, если есть
            if article_number:
                cursor.execute("""
                    INSERT INTO Laws (ArticleNumber, Title, Content)
                    VALUES (%s, %s, %s)
                    ON DUPLICATE KEY UPDATE Title=VALUES(Title), Content=VALUES(Content)
                """, (article_number, title, content.strip()))
                conn.commit()
                print(f"Добавлена статья {article_number}: {title}")

            article_number = match.group(1)
            title = text
            content = ''
            collecting = True
        else:
            collecting = False
    elif tag.name == 'p' and collecting:
        content += tag.get_text(strip=True) + '\n'

# Сохраняем последнюю статью
if article_number:
    cursor.execute("""
        INSERT INTO Laws (ArticleNumber, Title, Content)
        VALUES (%s, %s, %s)
        ON DUPLICATE KEY UPDATE Title=VALUES(Title), Content=VALUES(Content)
    """, (article_number, title, content.strip()))
    conn.commit()
    print(f"Добавлена статья {article_number}: {title}")

cursor.close()
conn.close()
print("Парсинг завершен.")
