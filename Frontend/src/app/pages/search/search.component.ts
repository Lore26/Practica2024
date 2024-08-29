import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {Booking, ISearchTrain, IStation, ITravelObject, TrainAppBookingPassengers} from 'src/app/models/Station';
import {StationsService} from 'src/app/services/stations.service';
import {TrainsService} from 'src/app/services/trains.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  stationMap = new Map<number, string>();

  searchObj: ITravelObject = {} as ITravelObject;
  stationList: IStation[] = [];
  trainsList: ISearchTrain[] = [];

  bookingObj: Booking = new Booking();
  bookingPassengers: TrainAppBookingPassengers = new TrainAppBookingPassengers();

  loggedUserData: any;

  constructor(private activatedRoute: ActivatedRoute, private stationSrv: StationsService, private trainSrv: TrainsService) {
    this.activatedRoute.params.subscribe((paramObj: any) => {
      debugger;
      this.searchObj.fromStationId = paramObj.fromStationId;
      this.searchObj.toStationId = paramObj.toStationId;
      this.searchObj.dateOfTravel = paramObj.dateOfTravel;
      this.bookingObj.travelDate = this.searchObj.dateOfTravel
    })
    const localData = localStorage.getItem('trainUser');
    if (localData != null) {
      this.loggedUserData = JSON.parse(localData);
      this.bookingObj.passengerId = this.loggedUserData.passengerID;
    }
  }

  ngOnInit(): void {
    this.loadStations();
  }

  private loadStations() {
    this.stationSrv.getAllStations().then((data) => {
      this.stationList = data || [];
      this.stationList.forEach((station) => {
        this.stationMap.set(station.id, station.name);
      });
    });
  }

  async onSearch() {
    this.trainsList = await this.trainSrv.getAvailableTrains(this.searchObj) || [];
  }

  protected readonly alert = alert;
}
