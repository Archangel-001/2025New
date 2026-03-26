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
    private ICondition _condition; // условие срабатывания
    private List<IEffect> _effects;
    private bool _isOneTime;
    private bool _fired = false; // чек для одноразовых событий
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
    public string CurrentLocationId = "Медблок";
    public List<string> Inventory = new List<string>();
    public Dictionary<string, bool> Flags = new Dictionary<string, bool>();
    public List<string> Log = new List<string>();
    public void AddItem(string item)
    {
        if (!Inventory.Contains(item)) // чек на наличие
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
        return Flags.ContainsKey(flag) && Flags[flag]; // защита на случай неустановки флага
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

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

public class HasItemCondition : ConditionBase
{
    private string _item;
    public HasItemCondition(string item)
    {
        _item = item;
    }
    public override bool Check(GameState state)
    {
        return state.HasItem(_item);
    }
}

public class FlagCondition : ConditionBase
{
    private string _flag;
    private bool _expected;
    public FlagCondition(string flag, bool expected = true)
    {
        _flag = flag;
        _expected = expected;
    }
    public override bool Check(GameState state)
    {
        return state.GetFlag(_flag) == _expected;
    }
}

public class HealthCondition : ConditionBase
{
    private string _op;
    private int _value;
    public HealthCondition(string op, int value)
    {
        _op = op;
        _value = value;
    }
    public override bool Check(GameState state)
    {
        if (_op == "<=") return state.Health <= _value;
        if (_op == ">=") return state.Health >= _value;
        if (_op == "==") return state.Health == _value;
        return false;
    }
}

public class TurnCountCondition : ConditionBase
{
    private string _op;
    private int _value;
    public TurnCountCondition(string op, int value)
    {
        _op = op;
        _value = value;
    }
    public override bool Check(GameState state)
    {
        if (_op == ">=") return state.TurnCount >= _value;
        if (_op == "==") return state.TurnCount == _value;
        return false;
    }
}

public class AlwaysTrue : ConditionBase
{
    public override bool Check(GameState state)
    {
        return true;
    }
}

public class AndCondition : ConditionBase
{
    private ICondition _a;
    private ICondition _b;
    public AndCondition(ICondition a, ICondition b)
    {
        _a = a;
        _b = b;
    }
    public override bool Check(GameState state)
    {
        return _a.Check(state) && _b.Check(state);
    }
}

public class OrCondition : ConditionBase
{
    private ICondition _a;
    private ICondition _b;
    public OrCondition(ICondition a, ICondition b)
    {
        _a = a;
        _b = b;
    }
    public override bool Check(GameState state)
    {
        return _a.Check(state) || _b.Check(state);
    }
}

public class NotCondition : ConditionBase
{
    private ICondition _inner;
    public NotCondition(ICondition inner)
    {
        _inner = inner;
    }
    public override bool Check(GameState state)
    {
        return !_inner.Check(state);
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

public class AddItemEffect : EffectBase
{
    private string _item;
    public AddItemEffect(string item)
    {
        _item = item;
    }
    public override void Apply(GameState state)
    {
        state.AddItem(_item);
        state.AddLog("Получен предмет: " + _item);
    }
}

public class RemoveItemEffect : EffectBase
{
    private string _item;
    public RemoveItemEffect(string item)
    {
        _item = item;
    }
    public override void Apply(GameState state)
    {
        state.RemoveItem(_item);
    }
}

public class SetFlagEffect : EffectBase
{
    private string _flag;
    private bool _value;
    public SetFlagEffect(string flag, bool value = true)
    {
        _flag = flag;
        _value = value;
    }
    public override void Apply(GameState state)
    {
        state.SetFlag(_flag, _value);
    }
}

public class DamageEffect : EffectBase
{
    private int _amount;
    public DamageEffect(int amount)
    {
        _amount = amount;
    }
    public override void Apply(GameState state)
    {
        state.Damage(_amount);
        state.AddLog("Получен урон: " + _amount);
    }
}

public class HealEffect : EffectBase
{
    private int _amount;
    public HealEffect(int amount)
    {
        _amount = amount;
    }
    public override void Apply(GameState state)
    {
        state.Heal(_amount);
        state.AddLog("Восстановлено здоровье: " + _amount);
    }
}

public class LogEffect : EffectBase
{
    private string _message;
    public LogEffect(string message)
    {
        _message = message;
    }
    public override void Apply(GameState state)
    {
        state.AddLog(_message);
    }
}

// добавить эффекты на концовки и локации

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

public class OnEnterLocationEvent : GameEventBase
{
    public OnEnterLocationEvent(ICondition condition, List<IEffect> effects, bool isOneTime = false)
        : base(condition, effects, isOneTime) { }
}
public class OnTurnEvent : GameEventBase
{
    public OnTurnEvent(ICondition condition, List<IEffect> effects)
        : base(condition, effects, false) { }
}
public class OneTimeEvent : GameEventBase
{
    public OneTimeEvent(ICondition condition, List<IEffect> effects)
        : base(condition, effects, true) { }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

