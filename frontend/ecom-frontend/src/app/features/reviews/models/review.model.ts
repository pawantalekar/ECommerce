export interface Review {
    id: string;
    productId: string;
    userId: string;
    rating: number;
    comment: string | null;
    createdAt: string;
    updatedAt: string | null;
    userName: string;
}