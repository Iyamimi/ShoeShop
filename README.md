##  Структура проекта
### Assignment – Задания и требования
Содержит документы с описанием задач и требований к проекту.

### Database – База данных и скрипты
- **Data**: Папка для хранения данных (CSV, XLS)
- **Diagrams**: Диаграммы базы данных
  - `ShoeshopDb.png` – Визуальная схема БД
- **Scripts**: SQL-скрипты
  - **Table_Scripts**: Скрипты создания таблиц
  - `ShoeshopDb.sql` – Полный скрипт создания базы данных

### Shoeshop – Основное решение Visual Studio
- **ShoeshopLibrary**: Библиотека классов
- **ShoeshopWeb**: Веб-приложение
- **ShoeshopWebApi**: API-слой проекта
- **ShoeshopWpf**: Десктопное приложение на WPF
- `Shoeshop.sln` – Основной файл решения Visual Studio

### **Work_Reports** – Отчёты
- `Otchety_po_LR.docx` – Отчёт по лабораторным и практическим работам

## Запуск проекта
1. Развернуть БД из `Database/Scripts/ShoeshopDb.sql`
2. Настроить подключение в `appsettings.json`
3. Запустить `Shoeshop.sln`
