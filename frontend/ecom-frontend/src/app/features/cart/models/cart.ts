export interface CartItemDto {
    productId: string;
    name: string;
    slug: string;
    price: number;
    thumbnailUrl: string | null;
    quantity: number;
}

export interface CartDto {
    id: string;
    itemsCount: number;
    total: number;
    items: CartItemDto[];
}