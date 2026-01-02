export interface AddressDto {
    id: string;
    fullName: string;
    phone: string;
    addressLine1: string;
    addressLine2?: string;
    city: string;
    state: string;
    pincode: string;
    country: string;
    addressType: string;
    isDefault: boolean;
}

export interface AddAddressCommand {
    fullName: string;
    phone: string;
    addressLine1: string;
    addressLine2?: string;
    city: string;
    state: string;
    pincode: string;
    country: string;
    addressType: string;
    isDefault: boolean;
}
export interface UpdateAddressCommand {
    id?: string;
    fullName: string;
    phone: string;
    addressLine1: string;
    addressLine2?: string;
    city: string;
    state: string;
    pincode: string;
    country: string;
    addressType: string;
    isDefault: boolean;
}
