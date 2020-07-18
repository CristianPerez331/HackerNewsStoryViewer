import { TestBed, async } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { HackerNewsStoriesComponent } from './hacker-news-stories/hacker-news-stories.component';
import { ApiService } from './services/api.service';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';

describe('AppComponent', () => {
  const mockApiService = jasmine.createSpyObj("ApiService", ["getLatestNews"]);
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        FormsModule,
      ],
      declarations: [
        AppComponent,
        HackerNewsStoriesComponent
      ],
      providers: [
        { provide: ApiService, useValue: mockApiService }
      ]
    }).compileComponents();

    mockApiService.getLatestNews.and.returnValue(
      of({"totalResultCount":1,"totalPages":1,"data":[{"id":1,"author":"Cristian","title":"This is the title text","url":"https://url.url/url","timePublished":1595042599.0}],"hasError":false})
    );
  }));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('#topbar-text').textContent).toContain("Cristian Perez's Code Challenge Submission");
  });
});
