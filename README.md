# Student Helper API

## Описание
Student Helper API — это веб-приложение для студентов, предоставляющее доступ к учебным материалам, AI-ассистенту и системе публикаций. API использует современный стек технологий .NET с архитектурой CQRS через MediatR и интеграцию с AI GigaChat от Сбербанка.

## Основные возможности

### Аутентификация и авторизация
- Регистрация пользователей с хешированием паролей
- JWT-аутентификация с ролевой моделью (admin/user)
- Автоматическое получение и кэширование токенов из контекста HTTP

### Учебные материалы
- Добавление лекций по предметам (только для администраторов)
- Получение лекций по предмету
- Хранение внешних ссылок на учебные материалы

### AI-Ассистент (GigaChat)
- Интеграция с AI GigaChat
- Контекстные промпты для разных предметов:
  - Математика
  - Программирование
  - Лекции
  - Общие вопросы
- Сохранение истории диалогов
- Автоматическое управление сессиями

### Публикации
- Создание публикаций пользователями
- Просмотр всех публикаций
- Чтение конкретной публикации
- Поддержка разных типов публикаций

### Управление группами
- Создание учебных групп (администраторами)
- Привязка пользователей к группам

## Технологический стек

### Бэкенд
- **.NET 9** с ASP.NET Core
- **Entity Framework Core** с MySQL
- **MediatR** для реализации CQRS
- **JWT Bearer Authentication**
- **Docker** и Docker Compose

### База данных
- **MySQL 8.0**

### AI Интеграция
- **GigaChat API** от Сбербанка
- Автоматическое получение и обновление OAuth токенов

### Безопасность
- Хеширование паролей с использованием PBKDF2
- Валидация JWT токенов
- Ролевой контроль доступа

## Структура проекта

```
StudentHelperAPI/
├── Features/                    # Функциональные модули
│   ├── Admin/                  # Админ-функции
│   │   ├── AddGroup/           # Добавление групп
│   │   └── AddLectureOnSubject/ # Добавление лекций
│   ├── AI/                     # AI-интеграция
│   │   └── Send/               # Отправка сообщений AI
│   ├── Authentication/         # Аутентификация
│   │   ├── Auth/               # Вход в систему
│   │   └── Reg/                # Регистрация
│   └── User/                   # Пользовательские функции
│       ├── Lectures/           # Работа с лекциями
│       └── Publications/       # Публикации
├── Core/                       # Ядро приложения
│   ├── Abstractions/           # Интерфейсы
│   └── Common/                 # Общие утилиты
├── Infrastructure/             # Инфраструктура
│   └── Services/               # Сервисы (GigaChatService)
├── Models/                     # Модели данных
└── Program.cs                  # Точка входа
```

## Быстрый старт

### Предварительные требования
- Docker и Docker Compose
- .NET 8 SDK (для разработки)
- Ключи доступа к GigaChat API (получить тут https://developers.sber.ru/portal/products/gigachat-api)

### Запуск через Docker

1. Клонируйте репозиторий:
```bash
git clone <https://github.com/DaniarKamaev/StudentHelperAPI>
cd StudentHelperAPI
```

2. Настройте переменные окружения в `docker-compose.yml`:
```yaml
environment:
  - GigaChat__ClientId=ваш_client_id
  - GigaChat__ClientSecret=ваш_client_secret
  - GigaChat__Scope=GIGACHAT_API_PERS
```

3. Запустите приложение:
```bash
docker-compose up -d
```

4. Приложение будет доступно по адресам:
   - API: `http://localhost:8080`
   - Swagger UI: `http://localhost:8080/swagger`

### Разработка

1. Установите зависимости:
```bash
dotnet restore
```

2. Настройте соединение с БД в `appsettings.json`

3. Запустите приложение:
```bash
dotnet run
```

## API Endpoints

### Аутентификация
```
POST /helper/auth           # Вход в систему
POST /helper/reg           # Регистрация
```

### Лекции
```
GET  /helper/lectures/{subject}    # Получить лекции по предмету
POST /helper/admin/lecture/add    # Добавить лекцию (admin only)
```

### AI Чат
```
POST /helper/ai/chat       # Отправить сообщение AI
```

### Публикации
```
GET  /helper/publications          # Получить все публикации
GET  /helper/publications/{id}     # Получить конкретную публикацию
POST /helper/publications          # Создать публикацию
```

### Группы
```
POST /helper/admin/group/add       # Создать группу (admin only)
```

## Конфигурация

### Файл appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=HelperDb;user=root;password=rootpassword"
  },
  "Jwt": {
    "Key": "your-secret-key-here-minimum-32-chars",
    "Issuer": "StudentHelperAPI",
    "Audience": "StudentHelperAPI-Users",
    "ExpireMinutes": 60
  },
  "GigaChat": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Scope": "GIGACHAT_API_PERS"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Особенности реализации

### Безопасность
- **Хеширование паролей**: Используется PBKDF2 с 100,000 итерациями
- **JWT**: Токены содержат userId, email, role и другие claims
- **Валидация**: Автоматическая проверка токенов на каждом запросе
- **Роли**: Разделение прав между администраторами и пользователями

### AI Интеграция
- **Кэширование токенов**: Токены GigaChat кэшируются на 25 минут
- **Системные промпты**: Разные контексты для разных типов запросов
- **Обработка ошибок**: Детальное логирование ошибок AI сервиса
- **Таймауты**: Настройка таймаутов для HTTP запросов

### База данных
- **Миграции EF Core**: Автоматическое создание схемы БД
- **Отслеживание**: Использование AsNoTracking для read-only запросов
- **Индексы**: Рекомендуется добавить индексы на часто используемые поля

### Архитектура
- **CQRS через MediatR**: Разделение команд и запросов
- **Vertical Slice Architecture**: Разделение на слои (Core, Infrastructure, Features)
- **Dependency Injection**: Все сервисы регистрируются через DI контейнер

## Логирование

Приложение использует детальное логирование:
- Уровень Debug для разработки
- Логирование всех AI запросов и ответов
- Логирование ошибок БД
- Логирование аутентификации

## Мониторинг

### Health Checks
```
GET /health     # Проверка работоспособности API
```

### Swagger UI
Документация API доступна по адресу `/swagger` в режиме разработки.

## Миграции базы данных

```bash
# Создание миграции
dotnet ef migrations add InitialCreate

# Применение миграций
dotnet ef database update
```

## Вклад в проект

1. Форкните репозиторий
2. Создайте feature ветку (`git checkout -b feature/AmazingFeature`)
3. Зафиксируйте изменения (`git commit -m 'Add some AmazingFeature'`)
4. Запушьте в ветку (`git push origin feature/AmazingFeature`)
5. Откройте Pull Request

## Лицензия

Этот проект был сделан чтоб вычеркнуть ужаснейшую библиотеу колледжа на всегда.
## Поддержка

Для вопросов и поддержки:
- Создайте issue в репозитории
- Обратитесь к разработчикам проекта

---

**Примечание**: Для работы AI функционала необходим аккаунт в GigaChat API от Сбербанка с соответствующими credentials.
