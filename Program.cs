using System.Globalization;
using Residents.Models;
using Residents.Services;

var service = new ResidentService();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("УЧЁТ ЖИЛЬЦОВ МНОГОКВАРТИРНОГО ДОМА");
    Console.WriteLine("1. Добавить человека");
    Console.WriteLine("2. Редактировать человека");
    Console.WriteLine("3. Список людей");
    Console.WriteLine("4. Добавить квартиру");
    Console.WriteLine("5. Редактировать квартиру");
    Console.WriteLine("6. Список квартир");
    Console.WriteLine("7. Заселить человека");
    Console.WriteLine("8. Выселить человека");
    Console.WriteLine("9. История заселений");
    Console.WriteLine("10. Жильцы по квартирам");
    Console.WriteLine("11. Добавить коммунальный платёж");
    Console.WriteLine("12. Редактировать коммунальный платёж");
    Console.WriteLine("13. Список коммунальных платежей");
    Console.WriteLine("14. Общая сумма платежей по дому");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите действие: ");

    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                service.AddPerson(
                    ReadText("ФИО: "),
                    ReadText("Паспортные данные: "),
                    ReadText("Телефон: "));
                Console.WriteLine("Человек добавлен.");
                break;

            case "2":
                ShowPeople(service);
                service.EditPerson(
                    ReadInt("ID человека: "),
                    ReadText("Новое ФИО: "),
                    ReadText("Паспортные данные: "),
                    ReadText("Телефон: "));
                Console.WriteLine("Данные человека обновлены.");
                break;

            case "3":
                ShowPeople(service);
                break;

            case "4":
                service.AddFlat(
                    ReadText("Номер квартиры: "),
                    ReadInt("Количество комнат: "),
                    ReadDouble("Площадь квартиры, м²: "));
                Console.WriteLine("Квартира добавлена.");
                break;

            case "5":
                ShowFlats(service);
                service.EditFlat(
                    ReadInt("ID квартиры: "),
                    ReadText("Новый номер квартиры: "),
                    ReadInt("Количество комнат: "),
                    ReadDouble("Площадь квартиры, м²: "));
                Console.WriteLine("Данные квартиры обновлены.");
                break;

            case "6":
                ShowFlats(service);
                break;

            case "7":
                if (!service.People.Any() || !service.Flats.Any())
                {
                    Console.WriteLine("Сначала добавьте человека и квартиру.");
                    break;
                }

                ShowPeople(service);
                ShowFlats(service);
                service.MoveIn(
                    ReadInt("ID человека: "),
                    ReadInt("ID квартиры: "),
                    ReadDate("Дата заселения (ГГГГ-ММ-ДД): "));
                Console.WriteLine("Человек заселён.");
                break;

            case "8":
                ShowActiveSettings(service);
                service.MoveOut(
                    ReadInt("ID записи о заселении: "),
                    ReadDate("Дата выселения (ГГГГ-ММ-ДД): "));
                Console.WriteLine("Человек выселен.");
                break;

            case "9":
                ShowSettings(service);
                break;

            case "10":
                ShowFlats(service);
                foreach (var flat in service.Flats)
                {
                    var residents = service.Settings
                        .Where(s => s.FlatId == flat.Id && s.IsActive)
                        .ToList();

                    Console.WriteLine();
                    Console.WriteLine($"Квартира {flat.Number}: жильцов - {residents.Count}");

                    foreach (var resident in residents)
                        Console.WriteLine($"  {service.GetPersonName(resident.PersonId)}");
                }
                break;

            case "11":
                if (!service.Flats.Any())
                {
                    Console.WriteLine("Сначала добавьте квартиру.");
                    break;
                }

                ShowFlats(service);
                service.AddBill(
                    ReadInt("ID квартиры: "),
                    ReadText("Период (например, 10.2026): "),
                    ReadText("Вид услуги: "),
                    ReadDecimal("Сумма платежа: "),
                    ReadYesNo("Платёж оплачен? (д/н): "));
                Console.WriteLine("Коммунальный платёж добавлен.");
                break;

            case "12":
                ShowBills(service);
                service.EditBill(
                    ReadInt("ID платежа: "),
                    ReadInt("ID квартиры: "),
                    ReadText("Период: "),
                    ReadText("Вид услуги: "),
                    ReadDecimal("Сумма платежа: "),
                    ReadYesNo("Платёж оплачен? (д/н): "));
                Console.WriteLine("Коммунальный платёж обновлён.");
                break;

            case "13":
                ShowBills(service);
                break;

            case "14":
                Console.WriteLine($"Всего начислено по дому: {service.GetTotalBills():0.00} руб.");
                Console.WriteLine($"Оплачено: {service.GetPaidBills():0.00} руб.");
                Console.WriteLine($"Осталось оплатить: {service.GetUnpaidBills():0.00} руб.");

                foreach (var flat in service.Flats)
                {
                    var total = service.Bills
                        .Where(b => b.FlatId == flat.Id)
                        .Sum(b => b.Amount);

                    Console.WriteLine($"Квартира {flat.Number}: {total:0.00} руб.");
                }
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Неизвестная команда.");
                break;
        }
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static string ReadText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var value = Console.ReadLine()?.Trim();

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        Console.WriteLine("Поле не должно быть пустым.");
    }
}

static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);

        if (int.TryParse(Console.ReadLine(), out var value) && value > 0)
            return value;

        Console.WriteLine("Введите целое число больше нуля.");
    }
}

static double ReadDouble(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim().Replace(',', '.');

        if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            && value > 0)
            return value;

        Console.WriteLine("Введите число больше нуля.");
    }
}

static decimal ReadDecimal(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim().Replace(',', '.');

        if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            && value >= 0)
            return value;

        Console.WriteLine("Введите корректную сумму не меньше нуля.");
    }
}

static DateTime ReadDate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);

        if (DateTime.TryParseExact(
            Console.ReadLine(),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date))
            return date;

        Console.WriteLine("Введите дату в формате ГГГГ-ММ-ДД.");
    }
}

static bool ReadYesNo(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var value = Console.ReadLine()?.Trim().ToLowerInvariant();

        if (value is "д" or "да" or "y" or "yes")
            return true;

        if (value is "н" or "нет" or "n" or "no")
            return false;

        Console.WriteLine("Введите д или н.");
    }
}

static void ShowPeople(ResidentService service)
{
    Console.WriteLine();
    Console.WriteLine("ЛЮДИ");

    if (!service.People.Any())
    {
        Console.WriteLine("Записей пока нет.");
        return;
    }

    foreach (var p in service.People)
        Console.WriteLine($"ID: {p.Id} | {p.FullName} | Паспорт: {p.Passport} | Телефон: {p.Phone}");
}

static void ShowFlats(ResidentService service)
{
    Console.WriteLine();
    Console.WriteLine("КВАРТИРЫ");

    if (!service.Flats.Any())
    {
        Console.WriteLine("Записей пока нет.");
        return;
    }

    foreach (var f in service.Flats)
        Console.WriteLine($"ID: {f.Id} | Квартира {f.Number} | Комнат: {f.Rooms} | Площадь: {f.Area:0.##} м² | Жильцов: {service.GetResidentCount(f.Id)}");
}

static void ShowSettings(ResidentService service)
{
    Console.WriteLine();
    Console.WriteLine("ИСТОРИЯ ЗАСЕЛЕНИЙ");

    if (!service.Settings.Any())
    {
        Console.WriteLine("Записей пока нет.");
        return;
    }

    foreach (var s in service.Settings)
        Console.WriteLine($"ID: {s.Id} | {service.GetPersonName(s.PersonId)} | Квартира {service.GetFlatNumber(s.FlatId)} | Заселение: {s.MoveInDate:yyyy-MM-dd} | Выселение: {(s.MoveOutDate.HasValue ? s.MoveOutDate.Value.ToString("yyyy-MM-dd") : "Проживает")}");
}

static void ShowActiveSettings(ResidentService service)
{
    Console.WriteLine();
    Console.WriteLine("ПРОЖИВАЮЩИЕ ЖИЛЬЦЫ");

    var active = service.Settings.Where(s => s.IsActive).ToList();

    if (!active.Any())
    {
        Console.WriteLine("Сейчас нет активных записей о заселении.");
        return;
    }

    foreach (var s in active)
        Console.WriteLine($"ID записи: {s.Id} | {service.GetPersonName(s.PersonId)} | Квартира {service.GetFlatNumber(s.FlatId)} | С {s.MoveInDate:yyyy-MM-dd}");
}

static void ShowBills(ResidentService service)
{
    Console.WriteLine();
    Console.WriteLine("КОММУНАЛЬНЫЕ ПЛАТЕЖИ");

    if (!service.Bills.Any())
    {
        Console.WriteLine("Записей пока нет.");
        return;
    }

    foreach (var b in service.Bills)
        Console.WriteLine($"ID: {b.Id} | Квартира {service.GetFlatNumber(b.FlatId)} | Период: {b.Period} | Услуга: {b.ServiceName} | Сумма: {b.Amount:0.00} руб. | {(b.IsPaid ? "Оплачено" : "Не оплачено")}");
}
