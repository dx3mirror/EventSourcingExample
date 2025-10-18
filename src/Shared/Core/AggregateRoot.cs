namespace Shared.Core
{
    /// <summary>
    /// Базовый класс агрегата для Event Sourcing.
    /// Хранит идентификатор, версию, буфер новых событий и реализует реплей истории.
    /// </summary>
    public abstract class AggregateRoot<TId, TEvent>
    {
        /// <summary>
        /// Идентификатор агрегата (обычно Guid). Устанавливается в доменной логике.
        /// </summary>
        public TId Id { get; protected set; } = default!;

        /// <summary>
        /// Текущая подтверждённая версия агрегата (последняя зафиксированная в хранилище).
        /// Стартовое значение: -1 (пустой стрим).
        /// </summary>
        public int Version { get; private set; } = -1;

        // Буфер несохранённых (новых) доменных событий, созданных методом Emit.
        private readonly List<TEvent> _uncommitted = [];

        /// <summary>
        /// Неподтверждённые (новые) события текущей транзакции.
        /// Используются репозиторием для последующей записи в EventStore.
        /// </summary>
        public IReadOnlyCollection<TEvent> UncommittedEvents => _uncommitted.AsReadOnly();

        /// <summary>
        /// Ожидаемая версия для оптимистичной блокировки при записи.
        /// Равна текущей Version до коммита новых событий.
        /// </summary>
        public int ExpectedVersion => Version;

        /// <summary>
        /// Какая будет версия после фиксации всех новых событий (Version + Count).
        /// </summary>
        public int PendingVersion => Version + _uncommitted.Count;

        /// <summary>
        /// Реплей истории событий в агрегат.
        /// Применяет события по порядку и устанавливает финальную версию.
        /// </summary>
        /// <param name="history">Последовательность событий в порядке возрастания версии.</param>
        /// <param name="startingVersion">
        /// Версия, от которой начинаем (обычно -1 для пустого стрима
        /// или версия снапшота, если реплей идёт после него).
        /// </param>
        public void Replay(IEnumerable<TEvent> history, int startingVersion = -1)
        {
            Version = startingVersion;
            foreach (var e in history)
            {
                When(e);     // применяем событие к состоянию
                Version++;   // увеличиваем версию после каждого события
            }
            _uncommitted.Clear(); // после реплея буфер должен быть пуст
        }

        /// <summary>
        /// Сгенерировать новое доменное событие:
        /// 1) применить его к состоянию (When),
        /// 2) положить в буфер для последующей записи.
        /// </summary>
        protected void Emit(TEvent @event)
        {
            When(@event);
            _uncommitted.Add(@event);
        }

        /// <summary>
        /// Забрать пачку новых событий и очистить буфер.
        /// Репозиторий вызывает перед append в EventStore.
        /// </summary>
        public IReadOnlyList<TEvent> DequeueUncommitted()
        {
            var batch = _uncommitted.ToArray();
            _uncommitted.Clear();
            return batch;
        }

        /// <summary>
        /// Отметить, что N событий успешно зафиксированы в хранилище:
        /// поднимаем подтверждённую версию на committedCount.
        /// </summary>
        public void MarkCommitted(int committedCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(committedCount);
            if (committedCount == 0) return;
            Version += committedCount;
        }

        /// <summary>
        /// Полиморфное применение события к состоянию агрегата.
        /// Реализуется в конкретном агрегате (switch/паттерн-матч по типам событий).
        /// </summary>
        protected abstract void When(TEvent @event);
    }
}
