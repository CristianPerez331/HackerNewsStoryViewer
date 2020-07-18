import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';

import { HackerNewsStoriesComponent } from './hacker-news-stories.component';
import { ApiService } from '../services/api.service';
import { of } from 'rxjs';
import { NewsStoryData } from '../models/news-story-data.model';

describe('HackerNewsStoriesComponent', () => {
  let component: HackerNewsStoriesComponent;
  let fixture: ComponentFixture<HackerNewsStoriesComponent>;
  const mockApiService = jasmine.createSpyObj("ApiService", ["getLatestNews"]);

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HackerNewsStoriesComponent ],
      providers: [
        { provide: ApiService, useValue: mockApiService }
      ]
    })
    .compileComponents();

    mockApiService.getLatestNews.and.returnValue(
      of({"totalResultCount":1,"totalPages":1,"data":[{"id":1,"author":"Cristian","title":"This is the title text","url":"https://url.url/url","timePublished":1595042599.0}],"hasError":false})
    );
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HackerNewsStoriesComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  
  it('should set defaults correctly', () => {
    expect(component.page).toEqual(0);
    expect(component.pageSize).toEqual(10);
    expect(component.searchQuery).toEqual(null);
    expect(component.loading).toEqual(true);
  });

  it('should call ApiService', () => {
    fixture.detectChanges();
    expect(mockApiService.getLatestNews).toHaveBeenCalledWith(0, 10, null);
  });
  
  it('should change loading to false after it calls ApiService', () => {
    fixture.detectChanges();
    expect(component.loading).toEqual(false);
  });

  /* getPageButtonLabels Tests */
  const callGetPageButtonLabels = (page, totalPages) => {
    component.page = page;
    component.newsStories = new NewsStoryData();
    component.newsStories.totalPages = totalPages;
    return component.getPageButtonLabels();
  };
  
  it('should return 1 page button when there is no data', () => {
    const pageButtonLabels = callGetPageButtonLabels(0, 0);
    expect(pageButtonLabels).toEqual([0]);
  });

  it('should return correct 9 buttons when there are 9 total pages', () => {
    const pageButtonLabels = callGetPageButtonLabels(0, 9);
    expect(pageButtonLabels).toEqual([0,1,2,3,4,5,6,7,8]);
  });

  it('should return correct 10 buttons when there are 11 total pages and page on is 1', () => {
    const pageButtonLabels = callGetPageButtonLabels(1, 11);
    expect(pageButtonLabels).toEqual([0,1,2,3,4,5,6,7,8,9]);
  });

  it('should return correct 10 buttons when there are 20 total pages and page on is 10', () => {
    const pageButtonLabels = callGetPageButtonLabels(10, 20);
    expect(pageButtonLabels).toEqual([6,7,8,9,10,11,12,13,14,15]);
  });

  /* setPageAndUpdateNews Tests */
  it('should call ApiService and update page when setPageAndUpdateNews is called', () => {
    component.setPageAndUpdateNews(17);
    expect(component.page).toEqual(17);
    expect(mockApiService.getLatestNews).toHaveBeenCalledWith(17, 10, null);
  });

  /* searchLatestNews Tests */
  it('should set timer and set loading to true when searchLatestNews is called', fakeAsync(() => {
    component.searchQuery = "test";
    component.searchLatestNews();
    expect(component.loading).toEqual(true);
    tick(150);
    expect(mockApiService.getLatestNews).toHaveBeenCalledWith(0, 10, "test");
  }));

  /* DOM tests */
  /*it('', () => {
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    searchQuery
    expect(compiled.querySelector('#topbar-text')).toBeNull();
  });*/
  
  it('should update search query when search field is changed ', fakeAsync(() => {
    fixture.detectChanges();
    const element = fixture.nativeElement.querySelector('#search-field').nativeElement;
    element.focus();
    element.value = 'test';
    element.dispatchEvent(new Event('input'));
    tick();
    fixture.detectChanges();
    expect(component.searchQuery).toEqual("test");
  }));

  it('should update pageSize when the dropdown is changed', () => {
    component.searchQuery = "-1";
    fixture.detectChanges();
    const element = fixture.nativeElement.querySelector('#pagesize-dropdown');
    element.value = 'test';
    element.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    expect(component.searchQuery).toEqual("test");
  });
});
