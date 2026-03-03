# WhatLoll.Encoder

[![.NET](https://img.shields.io/badge/.NET-Framework%204.6.2%2B%20%7C%20.NET%20Core%203.1%2B%20%7C%20.NET%20Standard%202.0%2B-blue)](https://dotnet.microsoft.com)


##  О проекте

**WhatLoll.Encoder** - это легковесная библиотека для шифрования и дешифрования текста с использованием уникальных таблиц замен WhatLoll. Идеально подходит для:

-  **Игр и квестов** - создавайте зашифрованные подсказки
-  **Обучения** - покажите пример простого шифрования
-  **Личных записок** - скрывайте текст от случайных глаз
-  **Развлечения** - отправляйте друзьям "секретные" сообщения

> [!WARNING]
>  Это **НЕ криптографически стойкое шифрование**! Это простая замена символов для развлечения и обучения. Расшифровка не 100%

---

## Установка
| Nuget | GitHub Realese |
|-------|----------------|
| ```dotnet add package WhatLoll.Encoder```| [Latest Update](https://github.com/xachapyri-dev/WhatLoll-Encoder-csharp/releases/latest) |
##  Возможности

###  Две версии шифрования

| Версия | Описание | Пример |
|--------|----------|--------|
| **V1** | Только русские буквы (классическая) | `привет` → `πßи1в0у` |
| **V2** | Русские + английские + цифры | `hello 123` → `Н0m0 1_2_3_` |

###  Технические преимущества
-  **Мультифреймворковость** - работает везде
-  **Нулевые зависимости** - чистый .NET
-  **Высокая производительность** - O(n) сложность
-  **Автоопределение версии** - DecryptAuto()
-  **Полная документация** - XML комментарии

# Пример кода
``` csharp
using WhatLoll.Encoder;

// Исходный текст
string original = "Привет, мир!";

// Шифрование (рекомендуется V2)
string encrypted = WhatLoll.EncryptV2(original);
Console.WriteLine($"Зашифровано: {encrypted}");

// Дешифрование
string decrypted = WhatLoll.DecryptV2(encrypted);
Console.WriteLine($"Расшифровано: {decrypted}");
```

``` csharp
string text = "Hello World 123!";

string encrypted = WhatLoll.EncryptV2(text);
// Результат: "Н0mm0 2h0m 1_2_3_!"

string decrypted = WhatLoll.DecryptV2(encrypted);
// Результат: "Hello World 123!"
```

## API Документация

| Метод | Параметры | Возвращает | Описание |
|---------|---------------|---------------|------------|
| EncryptV1(string input) | Текст | string | Шифрование V1 |
| EncryptV2(string input) | Текст | string | Шифрование V2 (рекомендуется) |
| DecryptV1(string input) | Шифр | string | Дешифрование V1 |
| DecryptV2(string input) | Шифр | string | Дешифрование V2 |
| DecryptAuto(string input) | Шифр | string | Автоопределение версии |
| Encrypt(string input, WhatLollVersion version) | Текст + версия | string | Универсальное шифрование|
| Decrypt(string input, WhatLollVersion version) | Шифр + версия | string | Универсальное дешифрование |

## Вспомогательные методы
``` csharp
// Получить таблицу шифрования
string table = WhatLoll.GetEncryptionTable(WhatLollVersion.V2);

// Проверить поддержку символа
bool supported = WhatLoll.IsCharacterSupported('ё', WhatLollVersion.V2);

// Количество поддерживаемых символов
int count = WhatLoll.SupportedCharactersCount(WhatLollVersion.V2);
```
