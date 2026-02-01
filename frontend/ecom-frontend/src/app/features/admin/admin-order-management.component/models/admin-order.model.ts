export interface AdminOrder {
  orderNumber: string;
  customerName: string;
  email: string;
  createdAt: string;
  totalAmount: number;
  orderStatus: string;
  paymentStatus: string;
}

export interface PagedOrdersResponse {
  orders: AdminOrder[];
  total: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface OrderFilters {
  orderStatus?: string[];
  paymentStatus?: string[];
  fromDate?: Date;
  toDate?: Date;
  searchTerm?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface OrderStatusHistory {
  status: string;
  changedAt: Date;
  changedByUserName?: string;
  notes?: string;
}

export interface UpdateOrderStatusResponse {
  orderNumber: string;
  status: string;
  changedAt: Date;
}

export const ORDER_STATUSES = [
  'Pending',
  'Confirmed',
  'Processing',
  'Shipped',
  'OutForDelivery',
  'Delivered',
  'Cancelled',
  'Failed'
] as const;

export type OrderStatus = typeof ORDER_STATUSES[number];
