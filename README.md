# ✈️ Flight Aggregator API

**Flight Aggregator API** — это RESTful-сервис, агрегирующий данные о доступных авиаперелётах от разных провайдеров и предоставляющий:

- Поиск рейсов с фильтрацией и сортировкой
- Бронирование и отмену рейсов
- Аутентификацию через JWT
- Логирование и кэширование

---

### 🚀 Возможности

✅ Агрегация рейсов от нескольких провайдеров\
✅ Тестовые API размещены на [Wiremock Cloud](https://wiremock.cloud)\
✅ Фильтрация по дате, цене, пересадкам, авиакомпании\
✅ Сортировка по цене и дате\
✅ Бронирование и отмена бронирований\
✅ Кэширование запросов (MemoryCache)\
✅ Логирование (`Serilog`) и глобальная обработка исключений\
✅ JWT-аутентификация и Swagger авторизация\
✅ Полная архитектура по принципам чистой архитектуры\
---

## 🧱 Архитектура проекта

```text
FlightAggregator
│
├── Api                    # Точка входа, контроллеры, middleware, swagger
├── Application            # Бизнес-логика, use cases, валидация
├── Domain                 # Value objects, сущности, доменные сервисы
├── Infrastructure         # Интеграция с внешним миром (провайдеры, базы, кэш, логгеры)
└── Tests                  # Юнит-тесты
```

---

## 🔐 Аутентификация

- Используется JWT (JSON Web Token)
- Swagger UI поддерживает авторизацию
- По умолчанию login доступен по `POST /api/Auth/login`
- После получения токена — отправляйте его в заголовке `Authorization: Bearer <token>`

---

## 📦 Примеры запросов

### 🔍 Поиск рейсов

```
GET /api/Flight?origin=Москва&destination=Питер&departureTime=2025-06-01
```

### ✉️ Авторизация

```
POST /api/Auth/login
{
  "email": "user@example.com"
}
```

### 📌 Бронирование

```
POST /api/Booking
{
  "flightNumber": "AB123",
  "passengerName": "Иван Иванов",
  "passengerEmail": "ivan@email.com"
}
```

### ❌ Отмена

```
DELETE /api/Booking/{bookingId}
```

---

## 🧪 Запуск локально

```bash
dotnet restore
dotnet run --project src/FlightAggregator.Api
```

Либо через Docker:

```bash
docker build -t flight-aggregator .
docker run -p 5000:80 flight-aggregator
```

---

## ☁️ Деплой в облаке

Проект развернут на Render:
[🌐 Swagger UI (если открыт)](https://your-render-app.onrender.com/swagger)

> Настрой переменные окружения:

- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`

---

## 🧪 Тесты

- Тестирование `FakeFlightProviderA` (с ретраями)
- Тесты фильтрации и сортировки результатов поиска рейсов
- Тестирование авторизации (выдача и проверка JWT)
- Возможность легко расширять тесты через `xUnit`

Запуск:

```bash
dotnet test
```

---

## 🌐 Клиентское приложение (Frontend на React)

Для демонстрации работы API реализовано клиентское приложение на **React**:

- Удобный интерфейс для поиска и фильтрации рейсов
- Возможность бронирования и отмены рейсов
- Авторизация через JWT
- Интеграция с основным API и визуализация полученных данных

[🌐 Перейти к приложению](https://flight-aggregator.vercel.app/)

---

## 🙌 Авторы

Разработано как тестовое задание на позицию .NET Developer.\
Контакт: gospodinkorolev\@gmail.com

