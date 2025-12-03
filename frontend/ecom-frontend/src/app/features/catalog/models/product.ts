
// Full Product (from API)
export interface Product {
    id: string;
    name: string;
    slug: string;
    shortDescription: string | null;
    description: string | null;
    price: number;
    sku: string;
    stockQuantity: number;
    categoryName: string;
    brandName: string;
    isActive: boolean;
    isFeatured: boolean;
    imageUrls: string[];
    tags: string[];
}

// Payload to send the API 
export interface AddProductPayload {
    name: string;
    shortDescription?: string;
    description?: string;
    price: number;
    sku: string;
    stockQuantity: number;
    imageUrls: string[];
    tags: string[];
    categoryName: string;
    categorySlug?: string;
    brandName: string;
    brandLogoUrl?: string;
}