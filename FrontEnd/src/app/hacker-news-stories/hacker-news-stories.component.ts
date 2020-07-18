import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { NewsStoryData } from '../models/news-story-data.model';

@Component({
  selector: 'app-hacker-news-stories',
  templateUrl: './hacker-news-stories.component.html',
  styleUrls: ['./hacker-news-stories.component.scss']
})
export class HackerNewsStoriesComponent implements OnInit {

  newsStories: NewsStoryData;
  page: number = 0;
  pageSize: number = 10;
  searchQuery: string = null;
  loading: boolean = true;
  private typingTimeout: any;
  
  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.getLatestNews();
  }

  getLatestNews(): void {
    this.loading = true;
    this.apiService.getLatestNews(this.page, this.pageSize, this.searchQuery)
      .subscribe((data) => {
        this.newsStories = new NewsStoryData().deserialize(data);
        this.loading = false;
    });
  }

  searchLatestNews(): void {
    this.loading = true;
    this.page = 0;
    clearTimeout(this.typingTimeout);
    this.typingTimeout = setTimeout(() => {
      this.getLatestNews();
    }, 150);
  }

  getPageButtonLabels(): Array<number> {
    // if we dont know many total pages there should be then we should just return an array of 0
    if(!this.newsStories || this.newsStories.totalPages <= 1)
      return [0];
    
    // if there are less than 10 pages then we just want to show as many as there are
    if(this.newsStories.totalPages < 10)
      return [...Array(this.newsStories.totalPages).keys()];
    
      // we also want to only show 1 - 10 when the page count is less than 6
    if(this.page < 5)
      return [...Array(10).keys()];

    // if page has 5 more elements in front of it, we want to show it in the middle
    if(this.page + 5 < this.newsStories.totalPages)
      return [...Array(10).keys()].map(x => x + this.page - 4);

    // if not then we just show the 10 elements near the end of the list
    return [...Array(10).keys()].map(x => x + this.newsStories.totalPages - 10);
  }

  setPageAndUpdateNews(newPage: number): void {
    this.page = newPage;
    this.getLatestNews();
  }

}
