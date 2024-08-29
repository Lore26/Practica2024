export interface IStation {
  id: number;
  name: string;
  city: string;
  state: string;
  latitude: number;
  longitude: number;
}

export interface ISchedule {
  id: number;
  routeId: number;
  trainId: number;
  arrivalTime: string;
  departureTime: string;
  days: number;
}

export interface IRoute {
  scheduleId: string;
  id: number;
  trainId: number;
  departureStationId: number;
  arrivalStationId: number;
}

export interface ISearchTrain {
  trainId: number;
  trainNo: number;
  trainName: string;
  capacity: number;
  routeId: number;
  departureStationId: number;
  arrivalStationId: number;
  distance: number;
  duration: string;
  scheduleId: number;
  arrivalTime: string;
  departureTime: string;
}

export class IPassenger {
  passengerID: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  password: string;

  constructor() {
    this.passengerID = 0;
    this.firstName = '';
    this.lastName = '';
    this.email = '';
    this.phone = '';
    this.password = '';
  }
}

export class Booking {
  bookingId: number;
  trainId: number;
  passengerId: number;
  travelDate: string;
  bookingDate: Date;
  totalSeats: number;
  TrainAppBookingPassengers: TrainAppBookingPassengers[];

  constructor() {
    this.TrainAppBookingPassengers = [];
    this.bookingDate = new Date();
    this.bookingId = 0;
    this.passengerId = 0;
    this.totalSeats = 0;
    this.trainId = 0;
    this.travelDate = '';
  }
}

export class TrainAppBookingPassengers {
  bookingPassengerId: number;
  bookingId: number;
  passengerName: string;
  seatNo: number;
  age: number;

  constructor() {
    this.bookingPassengerId = 0;
    this.bookingId = 0;
    this.passengerName = '';
    this.seatNo = 0;
    this.age = 0;
  }
}

export interface ITravelObject {
  fromStationId: number;
  toStationId: number;
  dateOfTravel: string;
}
