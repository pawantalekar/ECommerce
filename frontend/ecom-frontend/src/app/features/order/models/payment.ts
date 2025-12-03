export interface InitiatePaymentResponse {
    razorpayOrderId: string;
    amount: number;
    keyId: string;
}