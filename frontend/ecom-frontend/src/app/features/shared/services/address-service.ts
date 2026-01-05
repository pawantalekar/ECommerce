import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AddAddressCommand, AddressDto, UpdateAddressCommand } from '../models/address-model';


@Injectable({
  providedIn: 'root'
})
export class AddressService {
 private apiUrl = `${environment.apiBaseUrl}/Auth/addresses`;

  constructor(private http: HttpClient) {}

  getMyAddresses(): Observable<AddressDto[]> {
    return this.http.get<AddressDto[]>(this.apiUrl);
  }

  addMyAddress(command: AddAddressCommand): Observable<AddressDto> {
    return this.http.post<AddressDto>(this.apiUrl, command);
  }
  updateMyAddress(id: string, command: UpdateAddressCommand): Observable<AddressDto> {
    return this.http.put<AddressDto>(`${this.apiUrl}/${id}`, command);
  }
  deleteMyAddress(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}


