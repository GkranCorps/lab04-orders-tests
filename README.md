# Лабораторная работа 4

Минимальная система заказов: 3 класса, 1 интерфейс. C# / .NET 8, NUnit, Moq, Coverlet.

## Правила

- Цена и количество положительны, заказ непустой.
- Обычный покупатель: без скидки; VIP: скидка 10%.
- Доставка: 300, если сумма после скидки меньше 5000; иначе бесплатно.
- Отказ оплаты или InvalidOperationException сервиса оплаты дают false.
- DemoPaymentGateway — учебная заглушка, реальных платежей нет.

## Запуск

```sh
dotnet restore Shop.Orders.sln
dotnet test Shop.Orders.sln --settings coverage.runsettings --collect:"XPlat Code Coverage" --results-directory artifacts/after
python scripts/coverage.py artifacts/after --enforce
```

Для повторного замера используйте пустой каталог результатов, чтобы не смешивать разные запуски. Python нужен только для проверки порога и HTML-отчёта; приложение и тесты написаны на C#.

Проект приложения — библиотека с бизнес-логикой, отдельный проект tests — модульные тесты. Интерфейс и база данных в задании не требуются.

## GitHub

Workflow `.github/workflows/ci.yml` запускается на push и pull_request. Он собирает решение, выполняет тесты и запрещает успешное завершение проверки, если покрытие строк или ветвей ниже 100%.

Чтобы запрещалось именно слияние, дополнительно требуется правило GitHub для main: Require a pull request before merging и Require status checks to pass, обязательная проверка build-test-coverage. Сам YAML не включает защиту ветки.

## История

Метка baseline указывает исходную версию с конкретной зависимостью DemoPaymentGateway и одним тестом. Рефакторинг заменяет поле на IPaymentGateway и передаёт зависимость через конструктор. Алгоритм Checkout сохранён.
