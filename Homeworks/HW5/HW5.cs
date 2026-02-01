// Часть 1
interface IDamageable
{
    void TakeDamage(int damage);
}
abstract class Character : IDamageable
{
    private string name;
    private int health;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    public abstract void Attack();
    public void Move()
    {
        Console.WriteLine($"{name} moves.");
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}

// Часть 2
class Warrior : Character
{
    public override void Attack()
    {
        Console.WriteLine($"{Name} swings a sword!");
    }
}
class Mage : Character
{
    public override void Attack()
    {
        Console.WriteLine($"{Name} casts a fireball!");
    }
}

// Часть 3
class Demo
{
    private Character[] characters;
    public Demo()
    {
        characters = new Character[2];
        
        var warrior = new Warrior();
        warrior.Name = "Georgian Priest";
        warrior.Health = 1953;
        
        var mage = new Mage();
        mage.Name = "Austrian Painter";
        mage.Health = 1939;
        
        characters[0] = warrior;
        characters[1] = mage;
    }
    public void Run()
    {
        foreach (var c in characters)
        {
            c.Attack();
        }
    }
}