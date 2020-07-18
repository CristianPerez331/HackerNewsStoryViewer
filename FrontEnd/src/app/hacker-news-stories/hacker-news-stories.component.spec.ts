import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { HackerNewsStoriesComponent } from './hacker-news-stories.component';
import { ApiService } from '../services/api.service';
import { of } from 'rxjs';
import { NewsStoryData } from '../models/news-story-data.model';
import { By, BrowserModule } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

describe('HackerNewsStoriesComponent', () => {
  let component: HackerNewsStoriesComponent;
  let fixture: ComponentFixture<HackerNewsStoriesComponent>;
  const mockApiService = jasmine.createSpyObj("ApiService", ["getLatestNews"]);

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [ FormsModule, BrowserModule ],
      declarations: [ HackerNewsStoriesComponent ],
      providers: [
        { provide: ApiService, useValue: mockApiService }
      ]
    })
    .compileComponents();

    mockApiService.getLatestNews.and.returnValue(
      of({"totalResultCount":1,"totalPages":1,"data":[{"id":1,"author":"Cristian","title":"This is the title text","url":"https://url.url/url","timePublished":1595042599.0}],"hasError":false})
    );
    
    fixture = TestBed.createComponent(HackerNewsStoriesComponent);
    component = fixture.componentInstance;
  }));

  it('should create the component', () => {
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
  it('should update search query when search field is changed ', fakeAsync(() => {
    component.searchQuery = "test";
    fixture.detectChanges();
    tick();
    let searchField: DebugElement = fixture.debugElement.query(By.css("#search-field"));
    expect(searchField.nativeElement.value).toEqual("test");
  }));

  it('should update dropdown when pageSize is changed', fakeAsync(() => {
    component.pageSize = -1;
    fixture.detectChanges();
    tick();
    let dropdown: DebugElement = fixture.debugElement.query(By.css("#pagesize-dropdown"));
    expect(dropdown.nativeElement.value).toEqual("-1");
  }));

  it('should set current page as selected', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    let pageButtonSelected: DebugElement = fixture.debugElement.query(By.css(".pagebutton-selected"));
    expect(pageButtonSelected.nativeElement.innerText).toEqual((component.page+1).toString());
  }));
  
  it('should update pageSize and set page to 0 when dropdown is changed ', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    let dropdown: DebugElement = fixture.debugElement.query(By.css("#pagesize-dropdown"));
    dropdown.nativeElement.value = -1;
    dropdown.nativeElement.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    expect(component.pageSize.toString()).toEqual("-1");
    expect(component.page).toEqual(0);
  }));

  it('should display correct story data', fakeAsync(() => {
    component.pageSize = -1;
    fixture.detectChanges();
    tick();
    let storyContainers: Array<DebugElement> = fixture.debugElement.queryAll(By.css(".story-container"));
    let title: DebugElement = storyContainers[0].query(By.css(".story-title"));
    let author: DebugElement = storyContainers[0].query(By.css(".story-author"));
    let link: DebugElement = storyContainers[0].query(By.css(".story-link"));
    expect(storyContainers.length).toEqual(1);
    expect(title.nativeElement.innerText).toEqual(component.newsStories.data[0].title);
    expect(author.nativeElement.innerText).toEqual(`Author: ${component.newsStories.data[0].author}`);
    expect(link.nativeElement.href).toEqual(component.newsStories.data[0].url);
  }));

  it('should not display url when it doesnt exist in the data', fakeAsync(() => {
    component.pageSize = -1;
    fixture.detectChanges();
    mockApiService.getLatestNews.and.returnValue(
      of({"totalResultCount":1,"totalPages":1,"data":[{"id":1,"author":"Cristian","title":"This is the title text","timePublished":1595042599.0}],"hasError":false})
    );
    component.getLatestNews();
    fixture.detectChanges();
    tick();
    let storyContainers: Array<DebugElement> = fixture.debugElement.queryAll(By.css(".story-container"));
    let link: DebugElement = storyContainers[0].query(By.css(".story-link"));
    expect(storyContainers.length).toEqual(1);
    expect(link).toBeNull();
  }));

  it('should not display story data when loading', fakeAsync(() => {
    // Note: we need an extra detect changes here because the initial one will run the onInit 
    // which will set loading to false, so we have to set it back
    fixture.detectChanges();
    component.loading = true;
    fixture.detectChanges();
    tick();
    let storyContainers: Array<DebugElement> = fixture.debugElement.queryAll(By.css(".story-container"));
    expect(storyContainers.length).toEqual(0);
  }));

  it('should display loading text when loading', fakeAsync(() => {
    // Note: we need an extra detect changes here because the initial one will run the onInit 
    // which will set loading to false, so we have to set it back
    fixture.detectChanges();
    component.loading = true;
    fixture.detectChanges();
    tick();
    let loadingText: DebugElement = fixture.debugElement.query(By.css("#loading-text"));
    expect(loadingText).not.toBeNull();
  }));

  it('should not display loading text when not loading', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    let loadingText: DebugElement = fixture.debugElement.query(By.css("#loading-text"));
    expect(loadingText).toBeNull();
  }));

  it('should display nothing found text when nothing was found', fakeAsync(() => {
    // Note: we need an extra detect changes here because the initial one will run the onInit 
    // which will reset the newsStories
    fixture.detectChanges();
    component.newsStories.data = [];
    component.searchQuery = "test";
    fixture.detectChanges();
    tick();
    let loadingText: DebugElement = fixture.debugElement.query(By.css("#no-results-text"));
    expect(loadingText).not.toBeNull();
  }));

  it('should not display pageSize dropdown when nothing was found', fakeAsync(() => {
    // Note: we need an extra detect changes here because the initial one will run the onInit 
    // which will reset the newsStories
    fixture.detectChanges();
    component.newsStories.data = [];
    component.searchQuery = "test";
    fixture.detectChanges();
    tick();
    let loadingText: DebugElement = fixture.debugElement.query(By.css("#pagesize-dropdown-container"));
    expect(loadingText).toBeNull();
  }));

  it('should not display nothing found text when something was found', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    let loadingText: DebugElement = fixture.debugElement.query(By.css("#no-results-text"));
    expect(loadingText).toBeNull();
  }));
});
