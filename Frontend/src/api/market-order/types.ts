export interface DomainOrder {
  id: string;
  organizationId: string;
  type: number;
  title: string;
  body: string;
  extraInfo: string;
  totalAmount: number;
  creationDate: string;
  isPaid: boolean;
  isDelivered: boolean;
  isDeleted: boolean;
  canPay: boolean;
}
