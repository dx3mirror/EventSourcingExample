using PaymentService.Contracts.Enums;
using PaymentService.Core.Events;
using Shared.Core.Events;
using Shared.Core;

namespace PaymentService.Core
{
    /// <summary>
    /// Кошелёк для ES: один владелец (OwnerId), один счёт (Id).
    /// Id — идентификатор счёта. Владелец фиксируется при Init и больше не меняется.
    /// </summary>
    public sealed class WalletAggregate : AggregateRoot<Guid, IDomainEvent>
    {
        public Guid OwnerId { get; private set; }    // владелец счёта (один-единственный)
        public decimal Balance { get; private set; }
        public bool Exists { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void CreateNew(Guid ownerId, DateTime nowUtc)
        {
            if (Exists) return;
            if (ownerId == Guid.Empty) throw new ArgumentException(null, nameof(ownerId));
            Id = Guid.NewGuid();              
            OwnerId = ownerId;
            Exists = true;

            Emit(new WalletCreated(ownerId, EnsureUtc(nowUtc)));
        }

        /// <summary>Инициализация счёта: задаём Id (счёт) и OwnerId (владелец).</summary>
        public void Init(Guid walletId, Guid ownerId)
        {
            if (Exists) return;
            if (walletId == Guid.Empty) throw new ArgumentException("walletId пуст", nameof(walletId));
            if (ownerId == Guid.Empty) throw new ArgumentException("ownerId пуст", nameof(ownerId));

            Id = walletId;
            OwnerId = ownerId;
            Exists = true;
        }

        /// <summary>Пополнить счёт на сумму.</summary>
        public void Deposit(decimal amount, DateTime now)
        {
            EnsureInitialized();
            EnsurePositive(amount);

            Emit(new BalanceChanged(
                Amount: amount,
                OperationType: OperationType.Deposit,
                CreatedAt: now));
        }

        /// <summary>Списать со счёта (баланс не должен уйти в минус).</summary>
        public void Withdraw(decimal amount, DateTime now)
        {
            EnsureInitialized();
            EnsurePositive(amount);
            if (Balance < amount) throw new InvalidOperationException("Недостаточно средств");

            Emit(new BalanceChanged(
                Amount: amount,
                OperationType: OperationType.Withdraw,
                CreatedAt: now));
        }

        /// <summary>Применение событий (и при реплее, и при Emit).</summary>
        protected override void When(IDomainEvent @event)
        {
            switch (@event)
            {
                case WalletCreated e:
                    // При реплее восстанавливаем владельца и факт существования
                    if (OwnerId == Guid.Empty) OwnerId = e.OwnerId;
                    Exists = true;
                    UpdatedAt = EnsureUtc(e.CreatedAt);
                    break;

                case BalanceChanged e:
                    Balance = e.OperationType switch
                    {
                        OperationType.Deposit => Balance + e.Amount,
                        OperationType.Withdraw => Balance - e.Amount,
                        _ => Balance
                    };
                    UpdatedAt = e.CreatedAt;
                    break;

                default:
                    throw new InvalidOperationException($"Неизвестное событие: {@event.GetType().Name}");
            }
        }

        // --- проверки ---
        private void EnsureInitialized()
        {
            if (!Exists || Id == Guid.Empty || OwnerId == Guid.Empty)
                throw new InvalidOperationException("Счёт не инициализирован. Сначала вызови Init(walletId, ownerId).");
        }

        private static void EnsurePositive(decimal amount)
        {
            if (amount <= 0m)
                throw new ArgumentOutOfRangeException(nameof(amount), "Сумма должна быть > 0.");
        }

        static DateTime EnsureUtc(DateTime dt)
            => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }
}
