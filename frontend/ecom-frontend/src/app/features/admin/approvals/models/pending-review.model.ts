export interface PendingReview {
    id: string;
    productId: string;
    productName: string;
    userId: string;
    userName: string;
    rating: number;
    comment: string | null;
    createdAt: string; 
}