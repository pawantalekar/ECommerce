import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface UserProfileDto {
    firstName: string;
    lastName: string;
    gender?: string;
    mobileNumber: string;
    email: string;
    role: string;
}

export interface UpdateMyProfileCommand {
    firstName: string;
    lastName: string;
    gender?: string;
    mobileNumber: string;
}
@Injectable({
    providedIn: 'root'
})
export class MyProfileService {
    private apiUrl = `${environment.apiBaseUrl}/Auth/myprofile`;
    private profileUpdatedSource = new BehaviorSubject<void>(undefined);
    profileUpdated$ = this.profileUpdatedSource.asObservable();


    constructor(private http: HttpClient) { }

    getMyProfile(): Observable<{ result: UserProfileDto }> {
        return this.http.get<{ result: UserProfileDto }>(this.apiUrl);
    }
    updateMyProfile(command: UpdateMyProfileCommand): Observable<{ result: UserProfileDto }> {
        return this.http.put<{ result: UserProfileDto }>(`${environment.apiBaseUrl}/Auth/updateprofile`, command);
    }
    notifyProfileUpdated() {
        this.profileUpdatedSource.next();
    }
}