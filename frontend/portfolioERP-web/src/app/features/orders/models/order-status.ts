export enum OrderStatus {
  Draft = 0,
  Confirmed = 1,
  Processing = 2,
  Shipped = 3,
  Completed = 4,
  Cancelled = 5
}

export function getOrderStatusLabel(status: OrderStatus): string {
  switch (status) {
    case OrderStatus.Draft:
      return 'Bozza';

    case OrderStatus.Confirmed:
      return 'Confermato';

    case OrderStatus.Processing:
      return 'In lavorazione';

    case OrderStatus.Shipped:
      return 'Spedito';

    case OrderStatus.Completed:
      return 'Completato';

    case OrderStatus.Cancelled:
      return 'Annullato';

    default:
      return 'Sconosciuto';
  }
}
