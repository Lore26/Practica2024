import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { IStation, ITravelObject } from 'src/app/models/Station';
import { StationsService } from 'src/app/services/stations.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  stationList: IStation[] = [];
  travelObj: ITravelObject = {
    fromStationId: 0,
    toStationId: 0,
    dateOfTravel: ''
  };

  constructor(private stationSrv: StationsService, private router: Router) {
  }

  async ngOnInit(): Promise<void> {
    await this.loadStations();
  }

  async loadStations() {
    this.stationList = await this.stationSrv.getAllStations() || [];
  }

  onSearch() {
    this.router.navigate(['/search', this.travelObj.fromStationId, this.travelObj.toStationId, this.travelObj.dateOfTravel]);
  }

  ngOnDestroy(): void {
    // Cleanup if needed
  }
}
