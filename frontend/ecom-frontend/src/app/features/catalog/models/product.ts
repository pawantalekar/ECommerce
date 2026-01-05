
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
    categoryId: string | null;
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

//payload to update product
export interface UpdateProductPayload {
    id: string;
    name: string;
    shortDescription?: string | null;
    description?: string | null;
    price: number;
    sku: string;
    stockQuantity: number;
    categoryId: string;
    brandId?: string | null;
    brandLogoUrl?: string | null;
    isActive: boolean;
    isFeatured: boolean;
    imageUrls: string[];
    tags: string[];
}

export interface ProductForEdit {
    id: string;
    name: string;
    shortDescription?: string | null;
    description?: string | null;
    price: number;
    sku: string;
    stockQuantity: number;
    categoryId: string;
    categoryName: string;
    brandId?: string | null;
    brandName?: string | null;
    brandLogoUrl?: string | null;
    isActive: boolean;
    isFeatured: boolean;
    imageUrls: string[];
    tags: string[];
}

export interface Category{
    id:string;
    name:string;
}
export interface Brand {
    id: string;
    name: string;
}