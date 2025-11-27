export interface ShippingAddress {
    fullName: string;
    phone: string;
    addressLine1: string;
    addressLine2?: string;
    city: string;
    state: string;
    pincode: string;
}

export interface CheckoutItem {
    productId: string;
    quantity: number;
}

export interface OrderItem {
    productId: string;
    productName: string;
    thumbnailUrl?: string;
    unitPrice: number;
    quantity: number;
}

export interface Order {
    id: string;
    orderNumber: string;
    totalAmount: number;
    paymentStatus: string;
    orderStatus: string;
    createdAt: Date;
    items: OrderItem[];
    shippingAddress: ShippingAddress;
}