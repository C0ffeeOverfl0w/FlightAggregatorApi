# FlightAggregatorApi

**FlightAggregatorApi** — это RESTful API для поиска и бронирования авиаперелётов из различных источников. Проект разработан с нуля с акцентом на архитектуру, расширяемость и тестируемость.

## ✈️ Основной функционал

- Агрегация рейсов из нескольких провайдеров (фейковых)
- Поиск с фильтрацией (по дате, цене, количеству пересадок, авиакомпании)
- Сортировка по различным параметрам
- Бронирование рейса через источник
- Кэширование популярных запросов
- Логирование всех входящих и исходящих запросов
- Обработка долгих и нестабильных ответов от провайдеров

## 🧱 Архитектура

Проект построен на основе принципов **Clean Architecture** и разделён на слои:

- `Api` — внешний слой (контроллеры, Swagger)
- `Application` — бизнес-логика, use cases, DTO
- `Domain` — сущности, интерфейсы, value objects
- `Infrastructure` — HTTP-клиенты, кэш, логгинг
- `Tests` — unit- и интеграционные тесты

## 🔧 Технологии

- **.NET 9**, ASP.NET Core
- **Swashbuckle** (Swagger UI)
- **Polly** (обработка сбоев и таймаутов)
- **MemoryCache / Redis** (кэширование)
- **Serilog** (логирование)
- **xUnit + Moq** (тестирование)

## 🚀 Запуск проекта

```bash
git clone https://github.com/your-username/FlightAggregatorApi.git
cd FlightAggregatorApi
dotnet build
dotnet run --project src/FlightAggregatorApi