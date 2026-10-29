using Residents.Models;

namespace Residents.Services;

public class ResidentService
{
    public List<Person> People { get; } = new();
    public List<Flat> Flats { get; } = new();
    public List<Setting> Settings { get; } = new();
    public List<Bill> Bills { get; } = new();

    private int _personId = 1;
    private int _flatId = 1;
    private int _settingId = 1;
    private int _billId = 1;

    public Person AddPerson(string fullName, string passport, string phone)
    {
        if (People.Any(p => p.Passport == passport))
            throw new ArgumentException("Человек с такими паспортными данными уже существует.");

        var person = new Person
        {
            Id = _personId++,
            FullName = fullName,
            Passport = passport,
            Phone = phone
        };

        People.Add(person);
        return person;
    }

    public void EditPerson(int id, string fullName, string passport, string phone)
    {
        var person = People.FirstOrDefault(p => p.Id == id)
            ?? throw new ArgumentException("Человек не найден.");

        if (People.Any(p => p.Id != id && p.Passport == passport))
            throw new ArgumentException("Человек с такими паспортными данными уже существует.");

        person.FullName = fullName;
        person.Passport = passport;
        person.Phone = phone;
    }

    public Flat AddFlat(string number, int rooms, double area)
    {
        if (Flats.Any(f => f.Number == number))
            throw new ArgumentException("Квартира с таким номером уже существует.");

        if (rooms < 1 || area <= 0)
            throw new ArgumentException("Количество комнат и площадь должны быть больше нуля.");

        var flat = new Flat
        {
            Id = _flatId++,
            Number = number,
            Rooms = rooms,
            Area = area
        };

        Flats.Add(flat);
        return flat;
    }

    public void EditFlat(int id, string number, int rooms, double area)
    {
        var flat = Flats.FirstOrDefault(f => f.Id == id)
            ?? throw new ArgumentException("Квартира не найдена.");

        if (Flats.Any(f => f.Id != id && f.Number == number))
            throw new ArgumentException("Квартира с таким номером уже существует.");

        if (rooms < 1 || area <= 0)
            throw new ArgumentException("Количество комнат и площадь должны быть больше нуля.");

        flat.Number = number;
        flat.Rooms = rooms;
        flat.Area = area;
    }

    public Setting MoveIn(int personId, int flatId, DateTime date)
    {
        if (!People.Any(p => p.Id == personId))
            throw new ArgumentException("Человек не найден.");

        if (!Flats.Any(f => f.Id == flatId))
            throw new ArgumentException("Квартира не найдена.");

        if (Settings.Any(s => s.PersonId == personId && s.IsActive))
            throw new ArgumentException("Этот человек уже проживает в квартире.");

        var setting = new Setting
        {
            Id = _settingId++,
            PersonId = personId,
            FlatId = flatId,
            MoveInDate = date
        };

        Settings.Add(setting);
        return setting;
    }

    public void MoveOut(int settingId, DateTime date)
    {
        var setting = Settings.FirstOrDefault(s => s.Id == settingId && s.IsActive)
            ?? throw new ArgumentException("Активная запись о заселении не найдена.");

        if (date < setting.MoveInDate)
            throw new ArgumentException("Дата выселения не может быть раньше даты заселения.");

        setting.MoveOutDate = date;
    }

    public Bill AddBill(int flatId, string period, string serviceName, decimal amount, bool isPaid)
    {
        if (!Flats.Any(f => f.Id == flatId))
            throw new ArgumentException("Квартира не найдена.");

        if (amount < 0)
            throw new ArgumentException("Сумма платежа не может быть отрицательной.");

        var bill = new Bill
        {
            Id = _billId++,
            FlatId = flatId,
            Period = period,
            ServiceName = serviceName,
            Amount = amount,
            IsPaid = isPaid
        };

        Bills.Add(bill);
        return bill;
    }

    public void EditBill(int id, int flatId, string period, string serviceName, decimal amount, bool isPaid)
    {
        var bill = Bills.FirstOrDefault(b => b.Id == id)
            ?? throw new ArgumentException("Коммунальный платёж не найден.");

        if (!Flats.Any(f => f.Id == flatId))
            throw new ArgumentException("Квартира не найдена.");

        if (amount < 0)
            throw new ArgumentException("Сумма платежа не может быть отрицательной.");

        bill.FlatId = flatId;
        bill.Period = period;
        bill.ServiceName = serviceName;
        bill.Amount = amount;
        bill.IsPaid = isPaid;
    }

    public string GetPersonName(int id) =>
        People.FirstOrDefault(p => p.Id == id)?.FullName ?? "Не найден";

    public string GetFlatNumber(int id) =>
        Flats.FirstOrDefault(f => f.Id == id)?.Number ?? "Не найдена";

    public decimal GetTotalBills() => Bills.Sum(b => b.Amount);

    public decimal GetPaidBills() => Bills.Where(b => b.IsPaid).Sum(b => b.Amount);

    public decimal GetUnpaidBills() => Bills.Where(b => !b.IsPaid).Sum(b => b.Amount);

    public int GetResidentCount(int flatId) =>
        Settings.Count(s => s.FlatId == flatId && s.IsActive);
}
