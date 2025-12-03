export interface AddToCartRequest {
    productId: string;
    quantity?: number;
}

export interface UpdateQuantityRequest {
    quantity: number;
}