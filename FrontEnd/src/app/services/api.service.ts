import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private httpClient: HttpClient) { }

  public getLatestNews(page: number, pageSize: number, searchQuery: string) {
    let url: string;
    if (!searchQuery || searchQuery === '')
      url = `https://localhost:5001/api/HackerNews?page=${page}&pageSize=${pageSize}`;
    else
      url = `https://localhost:5001/api/HackerNews?page=${page}&pageSize=${pageSize}&includes=${searchQuery}`;
      
    return this.httpClient.get(url);
  }

}
