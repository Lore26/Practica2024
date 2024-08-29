import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {environment} from 'src/environments/environment';
import {CONSTANT} from '../constant/constant';
import {IStation} from "../models/Station";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class StationsService {
  apiEndPoint: string = '';

  constructor(private http: HttpClient) {
    this.apiEndPoint = environment.ApiEndPoint;
  }

  async getAllStations(): Promise<IStation[] | undefined> {
      let response = this.http.get<IStation[]>(`${this.apiEndPoint}${CONSTANT.ENDPOINTS.GET_ALL_STATIONS}`).toPromise();
      return response || {} as IStation[];
  }
}
