import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {environment} from 'src/environments/environment';
import {IRoute, ISchedule, ISearchTrain, ITravelObject} from "../models/Station";
import {Observable} from "rxjs";
import {CONSTANT} from "../constant/constant";

@Injectable({
  providedIn: 'root'
})
export class TrainsService {
  apiEndPoint: string = '';

  constructor(private http: HttpClient) {
    this.apiEndPoint = environment.ApiEndPoint;
  }

  async getAvailableTrains(travelObject: ITravelObject): Promise<ISearchTrain[] | undefined> {
    const params = new HttpParams()
      .set('startStationId', travelObject.fromStationId)
      .set('endStationId', travelObject.toStationId)
      .set('travelDate', travelObject.dateOfTravel);

    let response = this.http.get<ISearchTrain[]>(`${this.apiEndPoint}${CONSTANT.ENDPOINTS.GET_TRAINS_BETWEEN_STATIONS}`, {params: params}).toPromise();
    return response || {} as ISearchTrain[];
  }

  async getAllTrains(): Promise<ISearchTrain[] | undefined> {
    let response = this.http.get<ISearchTrain[]>(`${this.apiEndPoint}${CONSTANT.ENDPOINTS.GET_ALL_TRAINS}`).toPromise();
    return response || {} as ISearchTrain[];
  }

  async getTrainById(trainId: number): Promise<ISearchTrain | undefined> {
    let response = this.http.get<ISearchTrain>(`${this.apiEndPoint}${CONSTANT.ENDPOINTS.GET_ALL_TRAINS}/${trainId}`).toPromise();
    return response || {} as ISearchTrain;
  }

  async getAllRoutes(): Promise<ISearchTrain[] | undefined> {
    let allRoutes = this.http.get<IRoute[]>(`${this.apiEndPoint}${CONSTANT.ENDPOINTS.GET_ALL_ROUTES}`);
    if (!allRoutes) {
      return Promise.resolve([]);
    }
    let allSchedules : ISchedule[] = [];
    let allTrains: ISearchTrain[] = [];



    let trains: ISearchTrain[] = [];
    allRoutes.subscribe((routes: IRoute[]) => {
      routes.forEach((route: IRoute) => {
        let schedule = this.http.get<ISchedule>(`${this.apiEndPoint}${CONSTANT.ENDPOINTS.GET_ALL_STATIONS}/${route.scheduleId}`).toPromise();

        // if (schedule != undefinedallSchedules.push(schedule);
      });
    });

    return undefined;

  }
}
