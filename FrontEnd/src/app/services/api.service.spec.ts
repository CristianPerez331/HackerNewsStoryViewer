import { TestBed, getTestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { ApiService } from './api.service';

describe('ApiService', () => {
  let injector: TestBed;
  let service: ApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ApiService]
    });
    injector = getTestBed();
    service = injector.get(ApiService);
    httpMock = injector.get(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call correct url with empty search text', () => {
    service.getLatestNews(7,8,"").subscribe();
    const req = httpMock.expectOne("https://localhost:5001/api/HackerNews?page=7&pageSize=8");
    expect(req.request.method).toBe("GET");
  });

  it('should call correct url with null search text', () => {
    service.getLatestNews(7,8,null).subscribe();
    const req = httpMock.expectOne("https://localhost:5001/api/HackerNews?page=7&pageSize=8");
    expect(req.request.method).toBe("GET");
  });

  it('should call correct url with valid search text', () => {
    service.getLatestNews(7,8,"texttt").subscribe();
    const req = httpMock.expectOne("https://localhost:5001/api/HackerNews?page=7&pageSize=8&includes=texttt");
    expect(req.request.method).toBe("GET");
  });

});
