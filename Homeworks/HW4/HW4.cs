using System;
public class Student
{
    protected string name;
    protected int age;
    protected string group;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public int Age
    {
        get { return age; }
        set { age = value; }
    }
    public string Group
    {
        get { return group; }
        set { group = value; }
    }
    public Student(string name, int age, string group)
    {
        this.name = name;
        this.Age = age;
        this.group = group;
    }
    public void Learn()
    {
        Console.WriteLine($"Студент по имени {name}, которому {age} лет, учится в группе {group}");
    }
}
public class Balakavr : Student
{
    public Balakavr(string name, int age, string group) : base(name, age, group) { }
    public void Ekzamen()
    {
        Console.WriteLine($"Студент по имени {Name} сдает экзамены для бакалавриата");
    }
}
public class Magistr : Student
{
    public Magistr(string name, int age, string group) : base(name, age, group) { }
    public void Zachita()
    {
        Console.WriteLine($"Студент по имени {Name} защищает магистерскую диссертацию");
    }
}