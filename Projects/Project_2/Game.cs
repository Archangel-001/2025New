// ===== ИНТЕРФЕЙСЫ =====

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

// ===== АБСТРАКТНЫЕ КЛАССЫ =====

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

// базовый класс событий — хранит условие, эффекты и логику одноразовости
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

// ===== СОСТОЯНИЕ ИГРЫ =====

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

    // инвентарь
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

    // флаги
    public void SetFlag(string flag, bool value = true)
    {
        Flags[flag] = value;
    }

    public bool GetFlag(string flag)
    {
        return Flags.ContainsKey(flag) && Flags[flag];
    }

    // здоровье
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

    // завершение и журнал
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

// ===== УСЛОВИЯ =====

// простые условия
public class HasItemCondition : ConditionBase
{
    private string _item;

    public HasItemCondition(string item) { _item = item; }

    public override bool Check(GameState state)
    {
        return state.HasItem(_item);
    }
}

public class FlagCondition : ConditionBase
{
    private string _flag;
    private bool _expected;

    public FlagCondition(string flag, bool expected = true) { _flag = flag; _expected = expected; }

    public override bool Check(GameState state)
    {
        return state.GetFlag(_flag) == _expected;
    }
}

public class HealthCondition : ConditionBase
{
    private string _op;
    private int _value;

    public HealthCondition(string op, int value) { _op = op; _value = value; }

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

    public TurnCountCondition(string op, int value) { _op = op; _value = value; }

    public override bool Check(GameState state)
    {
        if (_op == ">=") return state.TurnCount >= _value;
        if (_op == "==") return state.TurnCount == _value;
        return false;
    }
}

public class AlwaysTrue : ConditionBase
{
    public override bool Check(GameState state) { return true; }
}

// составные условия
public class AndCondition : ConditionBase
{
    private ICondition _a;
    private ICondition _b;

    public AndCondition(ICondition a, ICondition b) { _a = a; _b = b; }

    public override bool Check(GameState state)
    {
        return _a.Check(state) && _b.Check(state);
    }
}

public class OrCondition : ConditionBase
{
    private ICondition _a;
    private ICondition _b;

    public OrCondition(ICondition a, ICondition b) { _a = a; _b = b; }

    public override bool Check(GameState state)
    {
        return _a.Check(state) || _b.Check(state);
    }
}

public class NotCondition : ConditionBase
{
    private ICondition _inner;

    public NotCondition(ICondition inner) { _inner = inner; }

    public override bool Check(GameState state)
    {
        return !_inner.Check(state);
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== ЭФФЕКТЫ =====

// инвентарь
public class AddItemEffect : EffectBase
{
    private string _item;

    public AddItemEffect(string item) { _item = item; }

    public override void Apply(GameState state)
    {
        state.AddItem(_item);
        state.AddLog("Получен предмет: " + _item);
    }
}

public class RemoveItemEffect : EffectBase
{
    private string _item;

    public RemoveItemEffect(string item) { _item = item; }

    public override void Apply(GameState state)
    {
        state.RemoveItem(_item);
    }
}

// флаги
public class SetFlagEffect : EffectBase
{
    private string _flag;
    private bool _value;

    public SetFlagEffect(string flag, bool value = true) { _flag = flag; _value = value; }

    public override void Apply(GameState state)
    {
        state.SetFlag(_flag, _value);
    }
}

// здоровье
public class DamageEffect : EffectBase
{
    private int _amount;

    public DamageEffect(int amount) { _amount = amount; }

    public override void Apply(GameState state)
    {
        state.Damage(_amount);
        state.AddLog("Получен урон: " + _amount);
    }
}

public class HealEffect : EffectBase
{
    private int _amount;

    public HealEffect(int amount) { _amount = amount; }

    public override void Apply(GameState state)
    {
        state.Heal(_amount);
        state.AddLog("Восстановлено здоровье: " + _amount);
    }
}

// журнал
public class LogEffect : EffectBase
{
    private string _message;

    public LogEffect(string message) { _message = message; }

    public override void Apply(GameState state)
    {
        state.AddLog(_message);
    }
}

// перемещение
public class ChangeLocationEffect : EffectBase
{
    private string _locationId;

    public ChangeLocationEffect(string locationId) { _locationId = locationId; }

    public override void Apply(GameState state)
    {
        state.CurrentLocationId = _locationId;
    }
}

public class AddExitEffect : EffectBase
{
    private string _fromId;
    private string _direction;
    private string _toId;
    private Dictionary<string, Location> _locations;

    public AddExitEffect(string fromId, string direction, string toId, Dictionary<string, Location> locations)
    {
        _fromId = fromId;
        _direction = direction;
        _toId = toId;
        _locations = locations;
    }

    public override void Apply(GameState state)
    {
        if (_locations.ContainsKey(_fromId))
            _locations[_fromId].AddExit(_direction, _toId);
    }
}

// концовка
public class TriggerEndingEffect : EffectBase
{
    private string _endingId;

    public TriggerEndingEffect(string endingId) { _endingId = endingId; }

    public override void Apply(GameState state)
    {
        state.TriggerEnding(_endingId);
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== СОБЫТИЯ =====

// срабатывает при входе в локацию
public class OnEnterLocationEvent : GameEventBase
{
    public OnEnterLocationEvent(ICondition condition, List<IEffect> effects, bool isOneTime = false)
        : base(condition, effects, isOneTime) { }
}

// срабатывает каждый ход
public class OnTurnEvent : GameEventBase
{
    public OnTurnEvent(ICondition condition, List<IEffect> effects)
        : base(condition, effects, false) { }
}

// срабатывает ровно один раз
public class OneTimeEvent : GameEventBase
{
    public OneTimeEvent(ICondition condition, List<IEffect> effects)
        : base(condition, effects, true) { }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== ОБЪЕКТЫ ВЗАИМОДЕЙСТВИЯ =====

// вспомогательный класс для диалогов NPC
public class DialogueLine
{
    public ICondition Condition;
    public string Text;
    public List<IEffect> Effects;

    public DialogueLine(ICondition condition, string text, List<IEffect> effects = null)
    {
        Condition = condition;
        Text = text;
        Effects = effects ?? new List<IEffect>();
    }
}

// контейнер — открывается один раз, может требовать условие
public class Chest : IInteractable
{
    public string Id { get; }
    private string _name;
    private ICondition _condition;
    private List<IEffect> _effects;
    private string _lockedMessage;
    private bool _opened = false;

    public Chest(string id, string name, ICondition condition, List<IEffect> effects, string lockedMessage = "Заперто.")
    {
        Id = id;
        _name = name;
        _condition = condition;
        _effects = effects;
        _lockedMessage = lockedMessage;
    }

    public void Interact(GameState state)
    {
        if (_opened)
        {
            Console.WriteLine(_name + ": уже открыт и пуст.");
            return;
        }
        if (_condition != null && !_condition.Check(state))
        {
            Console.WriteLine(_lockedMessage);
            return;
        }
        foreach (var effect in _effects)
            effect.Apply(state);
        _opened = true;
    }
}

// дверь — блокирует переход до выполнения условия
public class Door : IInteractable
{
    public string Id { get; }
    private string _name;
    private ICondition _condition;
    private List<IEffect> _effects;
    private string _lockedMessage;
    private bool _opened = false;

    public Door(string id, string name, ICondition condition, List<IEffect> effects, string lockedMessage = "Дверь заперта.")
    {
        Id = id;
        _name = name;
        _condition = condition;
        _effects = effects;
        _lockedMessage = lockedMessage;
    }

    public void Interact(GameState state)
    {
        if (_opened)
        {
            Console.WriteLine(_name + ": уже открыта.");
            return;
        }
        if (!_condition.Check(state))
        {
            Console.WriteLine(_lockedMessage);
            return;
        }
        foreach (var effect in _effects)
            effect.Apply(state);
        _opened = true;
        Console.WriteLine(_name + ": открыта.");
    }
}

// ловушка — одноразовая, после срабатывания неактивна
public class Trap : IInteractable
{
    public string Id { get; }
    private string _name;
    private List<IEffect> _effects;
    private string _triggerMessage;
    private bool _triggered = false;

    public Trap(string id, string name, List<IEffect> effects, string triggerMessage = "Ловушка сработала!")
    {
        Id = id;
        _name = name;
        _effects = effects;
        _triggerMessage = triggerMessage;
    }

    public void Interact(GameState state)
    {
        if (_triggered)
        {
            Console.WriteLine(_name + ": уже не опасна.");
            return;
        }
        Console.WriteLine(_triggerMessage);
        foreach (var effect in _effects)
            effect.Apply(state);
        _triggered = true;
    }
}

// NPC — выбирает первую подходящую реплику по условию
public class NPC : IInteractable
{
    public string Id { get; }
    private string _name;
    private List<DialogueLine> _dialogues;

    public NPC(string id, string name, List<DialogueLine> dialogues)
    {
        Id = id;
        _name = name;
        _dialogues = dialogues;
    }

    public void Interact(GameState state)
    {
        foreach (var line in _dialogues)
        {
            if (line.Condition.Check(state))
            {
                Console.WriteLine(_name + ": " + line.Text);
                foreach (var effect in line.Effects)
                    effect.Apply(state);
                return;
            }
        }
        Console.WriteLine(_name + " молчит.");
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== ЛОКАЦИЯ =====

public class Location
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }

    private Dictionary<string, string> _exits = new Dictionary<string, string>();
    private List<IInteractable> _objects = new List<IInteractable>();
    private List<GameEventBase> _events = new List<GameEventBase>();

    public Location(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    // выходы
    public void AddExit(string direction, string locationId)
    {
        _exits[direction] = locationId;
    }

    public Dictionary<string, string> GetExits()
    {
        return _exits;
    }

    public bool HasExit(string direction)
    {
        return _exits.ContainsKey(direction);
    }

    public string GetExit(string direction)
    {
        return _exits[direction];
    }

    // объекты
    public void AddObject(IInteractable obj)
    {
        _objects.Add(obj);
    }

    public IInteractable FindObject(string id)
    {
        foreach (var obj in _objects)
            if (obj.Id == id) return obj;
        return null;
    }

    public List<IInteractable> GetObjects()
    {
        return _objects;
    }

    // события
    public void AddEvent(GameEventBase ev)
    {
        _events.Add(ev);
    }

    public void FireEvents(GameState state)
    {
        foreach (var ev in _events)
            ev.TryFire(state);
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== КВЕСТ =====

public class Quest
{
    public string Name { get; }
    public string Description { get; }
    public bool IsCompleted { get; private set; } = false;

    private ICondition _completionCondition;
    private ICondition _visibilityCondition;

    public Quest(string name, string description, ICondition completionCondition, ICondition visibilityCondition = null)
    {
        Name = name;
        Description = description;
        _completionCondition = completionCondition;
        _visibilityCondition = visibilityCondition;
    }

    public bool IsVisible(GameState state)
    {
        if (_visibilityCondition == null) return true;
        return _visibilityCondition.Check(state);
    }

    public void Update(GameState state)
    {
        if (!IsCompleted)
            IsCompleted = _completionCondition.Check(state);
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== КОМАНДЫ =====

// help — список команд
public class HelpCommand : CommandBase
{
    private List<ICommand> _commands;

    public override string Name => "help";

    public HelpCommand(List<ICommand> commands) { _commands = commands; }

    public override void Execute(GameState state, string[] args)
    {
        Console.WriteLine("--- Команды ---");
        foreach (var cmd in _commands)
            Console.WriteLine(cmd.Name);
    }
}

// look — описание текущей локации
public class LookCommand : CommandBase
{
    private Dictionary<string, Location> _locations;

    public override string Name => "look";

    public LookCommand(Dictionary<string, Location> locations) { _locations = locations; }

    public override void Execute(GameState state, string[] args)
    {
        var loc = _locations[state.CurrentLocationId];
        Console.WriteLine("=== " + loc.Name + " ===");
        Console.WriteLine(loc.Description);

        Console.WriteLine("Объекты: ");
        foreach (var obj in loc.GetObjects())
            Console.WriteLine("  - " + obj.Id);

        Console.WriteLine("Выходы: ");
        foreach (var exit in loc.GetExits())
            Console.WriteLine("  " + exit.Key + " -> " + exit.Value);
    }
}

// go — переход в другую локацию
public class GoCommand : CommandBase
{
    private Dictionary<string, Location> _locations;

    public override string Name => "go";

    public GoCommand(Dictionary<string, Location> locations) { _locations = locations; }

    public override void Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Куда идти? Пример: go север");
            return;
        }

        var loc = _locations[state.CurrentLocationId];
        string direction = args[0];

        if (!loc.HasExit(direction))
        {
            Console.WriteLine("Туда нельзя пройти.");
            return;
        }

        state.CurrentLocationId = loc.GetExit(direction);
        _locations[state.CurrentLocationId].FireEvents(state);
        Console.WriteLine("Вы перешли: " + direction);
    }
}

// interact — взаимодействие с объектом по id
public class InteractCommand : CommandBase
{
    private Dictionary<string, Location> _locations;

    public override string Name => "interact";

    public InteractCommand(Dictionary<string, Location> locations) { _locations = locations; }

    public override void Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("С чем взаимодействовать? Пример: interact балка");
            return;
        }

        var loc = _locations[state.CurrentLocationId];
        var obj = loc.FindObject(args[0]);

        if (obj == null)
        {
            Console.WriteLine("Объект не найден: " + args[0]);
            return;
        }

        obj.Interact(state);
    }
}

// inv — инвентарь
public class InvCommand : CommandBase
{
    public override string Name => "inv";

    public override void Execute(GameState state, string[] args)
    {
        if (state.Inventory.Count == 0)
        {
            Console.WriteLine("Инвентарь пуст.");
            return;
        }
        Console.WriteLine("--- Инвентарь ---");
        foreach (var item in state.Inventory)
            Console.WriteLine("  - " + item);
    }
}

// status — здоровье и счётчик ходов
public class StatusCommand : CommandBase
{
    public override string Name => "status";

    public override void Execute(GameState state, string[] args)
    {
        Console.WriteLine("Здоровье: " + state.Health);
        Console.WriteLine("Ход: " + state.TurnCount);
        Console.WriteLine("До взрыва: " + (60 - state.TurnCount) + " ходов");
    }
}

// quests — журнал заданий
public class QuestsCommand : CommandBase
{
    private List<Quest> _quests;

    public override string Name => "quests";

    public QuestsCommand(List<Quest> quests) { _quests = quests; }

    public override void Execute(GameState state, string[] args)
    {
        Console.WriteLine("--- Задания ---");
        foreach (var quest in _quests)
        {
            if (!quest.IsVisible(state)) continue;
            string status = quest.IsCompleted ? "[x]" : "[ ]";
            Console.WriteLine(status + " " + quest.Name + ": " + quest.Description);
        }
    }
}

// log — последние события
public class LogCommand : CommandBase
{
    public override string Name => "log";

    public override void Execute(GameState state, string[] args)
    {
        Console.WriteLine("--- Журнал ---");
        int start = Math.Max(0, state.Log.Count - 10);
        for (int i = start; i < state.Log.Count; i++)
            Console.WriteLine(state.Log[i]);
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== ИГРОВОЙ МИР =====

public class Game
{
    private GameState _state;
    private Dictionary<string, Location> _locations;
    private Dictionary<string, ICommand> _commands;
    private List<Quest> _quests;
    private List<GameEventBase> _globalEvents;

    public Game(GameState state, Dictionary<string, Location> locations,
                Dictionary<string, ICommand> commands, List<Quest> quests,
                List<GameEventBase> globalEvents)
    {
        _state = state;
        _locations = locations;
        _commands = commands;
        _quests = quests;
        _globalEvents = globalEvents;
    }

    // один ход игры
    public void ProcessTurn(string input)
    {
        string[] parts = input.Trim().ToLower().Split(' ');
        string commandName = parts[0];
        string[] args = new string[parts.Length - 1];
        for (int i = 1; i < parts.Length; i++)
            args[i - 1] = parts[i];

        if (_commands.ContainsKey(commandName))
            _commands[commandName].Execute(_state, args);
        else
            Console.WriteLine("Неизвестная команда. Введите help.");

        if (_state.IsGameOver) return;

        _state.TurnCount++;
        _locations[_state.CurrentLocationId].FireEvents(_state);

        foreach (var ev in _globalEvents)
            ev.TryFire(_state);

        foreach (var quest in _quests)
            quest.Update(_state);

        foreach (var message in _state.Log)
            Console.WriteLine(message);
        _state.Log.Clear();
    }

    // вывод концовки
    public void PrintEnding()
    {
        Console.WriteLine();
        switch (_state.EndingId)
        {
            case "Герой":
                Console.WriteLine("Вы починили реактор и вышли вместе с Балкиным. Комплекс спасён.");
                break;
            case "Беглец":
                Console.WriteLine("Вы сбежали через аварийный выход. Реактор взорвался. Балкин погиб.");
                break;
            case "Тугодум":
                Console.WriteLine("Время вышло. Реактор взорвался. Вы не успели.");
                break;
        }
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== СБОРКА МИРА =====

public static class WorldBuilder
{
    public static (Dictionary<string, Location> locations, List<Quest> quests, List<GameEventBase> globalEvents)
        Build()
    {
        var locations = new Dictionary<string, Location>();

        // медблок — стартовая локация
        // fix 3: добавлен фонарик в шкаф
        var medblok = new Location("Медблок", "Медблок",
            "Медицинский блок комплекса. Аварийное освещение мигает. На стене висит схема эвакуации.");
        medblok.AddObject(new Chest(
            "шкаф", "Аптечный шкаф",
            new AlwaysTrue(),
            new List<IEffect>
            {
                new AddItemEffect("Аптечка"),
                new AddItemEffect("Фонарик"),
                new AddItemEffect("Гаечный ключ")
            }
        ));
        // fix 4: вентиляция добавляет выход "север" — то же направление что проверяет GoCommand
        medblok.AddObject(new Door(
            "вентиляция", "Вентиляционная решётка",
            new HasItemCondition("Гаечный ключ"),
            new List<IEffect>
            {
                new LogEffect("Вы откручиваете решётку. Виден тёмный лаз на север."),
                new AddExitEffect("Медблок", "север", "Аварийный выход", locations)
            },
            "Решётка прикручена болтами. Нужен инструмент."
        ));
        locations["Медблок"] = medblok;

        // склад — новая промежуточная локация между медблоком и коридором
        // fix 5: усложнение карты — склад с карточкой У1
        var storage = new Location("Склад", "Склад",
            "Захламлённый склад оборудования. Стеллажи с ящиками, часть повалена. Тускло светит аварийный фонарь.");
        storage.AddObject(new Chest(
            "стеллаж", "Стеллаж с инструментами",
            new AlwaysTrue(),
            new List<IEffect> { new AddItemEffect("Карточка У1") }
        ));
        storage.AddObject(new Chest(
            "аптечка_склад", "Настенная аптечка",
            new AlwaysTrue(),
            new List<IEffect> { new AddItemEffect("Аптечка"), new LogEffect("Вы нашли ещё одну аптечку.") }
        ));
        locations["Склад"] = storage;

        // тёмный коридор
        // fix 1: балка — OnEnterLocationEvent (одноразовое), после падения становится обычным объектом
        // fix 6: урон от темноты снижен с 10 до 5
        var corridor = new Location("Коридор", "Тёмный коридор",
            "Длинный тёмный коридор. Аварийное освещение не работает. Что-то скрипит в темноте.");
        corridor.AddEvent(new OnEnterLocationEvent(
            new AlwaysTrue(),
            new List<IEffect>
            {
                new DamageEffect(15),
                new SetFlagEffect("BeamFell"),
                new LogEffect("Потолочная балка не выдерживает и падает! Вы получаете 15 урона.")
            },
            isOneTime: true
        ));
        // балка после падения — просто объект для осмотра, не активная ловушка
        corridor.AddObject(new Chest(
            "балка", "Упавшая балка",
            new FlagCondition("BeamFell"),
            new List<IEffect> { new LogEffect("Тяжёлая металлическая балка. Уже не опасна, но мешает пройти.") },
            "Балка ещё не упала."
        ));
        corridor.AddObject(new Chest(
            "ящик", "Ящик техника",
            new HasItemCondition("Карточка У1"),
            new List<IEffect> { new AddItemEffect("Карточка У2") },
            "Ящик заперт. Нужна карточка У1."
        ));
        // fix 6: урон 5 за ход вместо 10
        corridor.AddEvent(new OnTurnEvent(
            new NotCondition(new HasItemCondition("Фонарик")),
            new List<IEffect> { new DamageEffect(5), new LogEffect("Темнота дезориентирует вас. -5 HP") }
        ));
        locations["Коридор"] = corridor;

        // технический отсек — новая локация с графитовым наконечником
        // fix 5: усложнение карты
        var techRoom = new Location("Техотсек", "Технический отсек",
            "Небольшое помещение с трубопроводами и щитками управления. Пахнет горелой изоляцией.");
        techRoom.AddObject(new Chest(
            "щиток", "Технический щиток",
            new HasItemCondition("Карточка У2"),
            new List<IEffect>
            {
                new AddItemEffect("Графитовый наконечник"),
                new LogEffect("За щитком нашёлся графитовый наконечник.")
            },
            "Щиток заперт на карточку У2."
        ));
        locations["Техотсек"] = techRoom;

        // реакторный зал
        var reactor = new Location("Реакторный зал", "Реакторный зал",
            "Огромное помещение. В центре — повреждённый реактор. Панель управления мигает красным.");
        reactor.AddObject(new Chest(
            "панель", "Панель реактора",
            new AlwaysTrue(),
            new List<IEffect>
            {
                new AddItemEffect("Боровый стержень"),
                new SetFlagEffect("ReactorChecked"),
                new LogEffect("Вы осмотрели реактор. Нужны: Боровый стержень + Графитовый наконечник.")
            }
        ));
        reactor.AddObject(new Door(
            "реактор", "Реактор",
            new AndCondition(
                new HasItemCondition("Боровый стержень"),
                new HasItemCondition("Графитовый наконечник")
            ),
            new List<IEffect>
            {
                new RemoveItemEffect("Боровый стержень"),
                new RemoveItemEffect("Графитовый наконечник"),
                new SetFlagEffect("ReactorFixed"),
                new AddExitEffect("Реакторный зал", "юг", "Главный гейт", locations),
                new LogEffect("Реактор стабилизирован! Путь к шлюзу открыт.")
            },
            "Не хватает компонентов для ремонта."
        ));
        locations["Реакторный зал"] = reactor;

        // аварийный выход — сюда ведёт вентиляция из медблока
        var emergency = new Location("Аварийный выход", "Аварийный выход",
            "Узкий вентиляционный лаз выводит за периметр комплекса.");
        emergency.AddEvent(new OnEnterLocationEvent(
            new AlwaysTrue(),
            new List<IEffect> { new TriggerEndingEffect("Беглец") },
            isOneTime: true
        ));
        locations["Аварийный выход"] = emergency;

        // главный гейт
        var gate = new Location("Главный гейт", "Главный гейт",
            "Массивная бронедверь эвакуационного шлюза. За толстым стеклом видно небо.");
        gate.AddObject(new Door(
            "шлюз", "Эвакуационный шлюз",
            new HasItemCondition("Карточка У3"),
            new List<IEffect> { new TriggerEndingEffect("Герой") },
            "Нужна карточка У3."
        ));
        locations["Главный гейт"] = gate;

        // связи между локациями
        medblok.AddExit("восток", "Склад");
        storage.AddExit("запад", "Медблок");
        storage.AddExit("север", "Коридор");
        corridor.AddExit("юг", "Склад");
        corridor.AddExit("восток", "Реакторный зал");
        corridor.AddExit("запад", "Техотсек");
        techRoom.AddExit("восток", "Коридор");
        reactor.AddExit("запад", "Коридор");
        // выход "север" из медблока добавляется через вентиляцию динамически

        // fix 2: Балкин — реплика "спасибо" убрана, лечение и выдача предметов в одном взаимодействии
        // порядок проверок: сначала "уже вылечен и предметы ещё не взяты", потом "есть аптечка", потом дефолт
        corridor.AddObject(new NPC(
            "балкин", "Балкин",
            new List<DialogueLine>
            {
                new DialogueLine(
                    new AndCondition(
                        new FlagCondition("BalkinHealed"),
                        new NotCondition(new FlagCondition("BalkinGaveItems"))
                    ),
                    "Спасибо... держи карточку У3 и наконечник. Удачи.",
                    new List<IEffect>
                    {
                        new AddItemEffect("Карточка У3"),
                        new AddItemEffect("Графитовый наконечник"),
                        new SetFlagEffect("BalkinGaveItems")
                    }
                ),
                new DialogueLine(
                    new FlagCondition("BalkinGaveItems"),
                    "Иди. У тебя мало времени."
                ),
                new DialogueLine(
                    new HasItemCondition("Аптечка"),
                    "Аптечка... пожалуйста...",
                    new List<IEffect>
                    {
                        new RemoveItemEffect("Аптечка"),
                        new SetFlagEffect("BalkinHealed"),
                        new LogEffect("Вы вылечили Балкина. Он благодарен.")
                    }
                ),
                new DialogueLine(
                    new AlwaysTrue(),
                    "Мне нужна аптечка... найди её в медблоке или на складе."
                )
            }
        ));

        // квесты
        var quests = new List<Quest>
        {
            new Quest(
                "Оценить ущерб",
                "Осмотреть панель реактора.",
                new FlagCondition("ReactorChecked")
            ),
            new Quest(
                "Стабилизировать реактор",
                "Найти компоненты и починить реактор.",
                new FlagCondition("ReactorFixed"),
                new FlagCondition("ReactorChecked")
            ),
            new Quest(
                "Выбраться из комплекса",
                "Найти путь на поверхность.",
                new OrCondition(new FlagCondition("Escaped"), new FlagCondition("ReactorFixed"))
            )
        };

        // глобальные события — таймер реактора
        var globalEvents = new List<GameEventBase>
        {
            new OneTimeEvent(
                new TurnCountCondition(">=", 40),
                new List<IEffect> { new LogEffect("⚠ ВНИМАНИЕ: до взрыва реактора осталось 20 ходов!") }
            ),
            new OneTimeEvent(
                new AndCondition(
                    new TurnCountCondition(">=", 60),
                    new NotCondition(new FlagCondition("ReactorFixed"))
                ),
                new List<IEffect> { new TriggerEndingEffect("Тугодум"), new LogEffect("Реактор взорвался.") }
            )
        };

        return (locations, quests, globalEvents);
    }
}

//LMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAOLMAO

// ===== ТОЧКА ВХОДА =====

var state = new GameState();
var (locations, quests, globalEvents) = WorldBuilder.Build();

// регистрация команд
var commandList = new List<ICommand>();
var commands = new Dictionary<string, ICommand>();

void Register(ICommand cmd) { commandList.Add(cmd); commands[cmd.Name] = cmd; }

Register(new HelpCommand(commandList));
Register(new LookCommand(locations));
Register(new GoCommand(locations));
Register(new InteractCommand(locations));
Register(new InvCommand());
Register(new StatusCommand());
Register(new QuestsCommand(quests));
Register(new LogCommand());

var game = new Game(state, locations, commands, quests, globalEvents);

// вступление
Console.WriteLine("=== КОМПЛЕКС «АРХАНГЕЛ» ===");
Console.WriteLine("Реактор повреждён. У вас 60 ходов. Введите help.");
Console.WriteLine();

// главный цикл
while (!state.IsGameOver)
{
    Console.Write("> ");
    string input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;
    game.ProcessTurn(input);
}

game.PrintEnding();