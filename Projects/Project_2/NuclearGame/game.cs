public interface ICondition
{
    bool Check(GameState state);
}
public interface IEffect
{
    void Apply(GameState state);
}
public interface ICommand
{
    string Name { get; }
    void Execute(GameState state, string[] args);
}
public interface IInteractable
{
    string Id { get; }
    void Interact(GameState state);
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

public abstract class ConditionBase : ICondition
{
    public abstract bool Check(GameState state);
}
public abstract class EffectBase : IEffect
{
    public abstract void Apply(GameState state);
}
public abstract class CommandBase : ICommand
{
    public abstract string Name { get; }
    public abstract void Execute(GameState state, string[] args);
}
public abstract class GameEventBase
{
    private ICondition _condition;
    private List<IEffect> _effects;
    private bool _isOneTime;
    private bool _fired = false;
    protected GameEventBase(ICondition condition, List<IEffect> effects, bool isOneTime = false)
    {
        _condition = condition;
        _effects = effects;
        _isOneTime = isOneTime;
    }
    public void TryFire(GameState state)
    {
        if (_isOneTime && _fired) return;
        if (!_condition.Check(state)) return;
        foreach (var effect in _effects)
            effect.Apply(state);

        _fired = true;
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

public class GameState
{
    public int Health = 100;
    public bool IsAlive => Health > 0;
    public bool IsGameOver = false;
    public string EndingId = "";
    public int TurnCount = 0;
    public string CurrentLocationId = "Лаборатория";
    public List<string> Inventory = new List<string>();
    public Dictionary<string, bool> Flags = new Dictionary<string, bool>();
    public List<string> Log = new List<string>();
    public void AddItem(string item)
    {
        if (!Inventory.Contains(item))
            Inventory.Add(item);
    }
    public void RemoveItem(string item)
    {
        Inventory.Remove(item);
    }
    public bool HasItem(string item)
    {
        return Inventory.Contains(item);
    }
    public void SetFlag(string flag, bool value = true)
    {
        Flags[flag] = value;
    }
    public bool GetFlag(string flag)
    {
        return Flags.ContainsKey(flag) && Flags[flag];
    }
    public void Damage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Health = 0;
            TriggerEnding("Тугодум");
        }
    }
    public void Heal(int amount)
    {
        Health += amount;
        if (Health > 100)
            Health = 100;
    }
    public void TriggerEnding(string endingId)
    {
        IsGameOver = true;
        EndingId = endingId;
    }
    public void AddLog(string message)
    {
        Log.Add(message);
    }
}