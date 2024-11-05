import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-breadcrumbs',
  templateUrl: './breadcrumbs.component.html',
  styleUrls: ['./breadcrumbs.component.css']
})
export class BreadcrumbsComponent implements OnInit, OnDestroy {
  private ROUTE_DATA_BREADCRUMB = 'breadcrumb';
  public crumbs: MenuItem[] | undefined;
  public home: MenuItem | undefined;
  private _breadcrumbsSubscription: Subscription | undefined;

  constructor(private route: ActivatedRoute, private router: Router) { }
  ngOnDestroy(): void {
    this._breadcrumbsSubscription?.unsubscribe();
  }

  ngOnInit() {
    this.home = { icon: 'pi pi-home', routerLink: '/' };
    this.router.events
    .pipe(filter(event => event instanceof NavigationEnd))
    .subscribe(() => this.crumbs = this.createBreadcrumbs(this.route.root));
  }

  private createBreadcrumbs(route: ActivatedRoute, url: string = '', breadcrumbs: MenuItem[] = []): MenuItem[] | undefined {
    const children: ActivatedRoute[] = route.children;

    if (children.length === 0) {
      return breadcrumbs;
    }

    for (const child of children) {
      const routeURL: string = child.snapshot.url.map(segment => segment.path).join('/');
      if (routeURL !== '') {
        url += `/${routeURL}`;
      }

      const label = !child?.snapshot?.routeConfig?.data ? undefined : child.snapshot.routeConfig.data[this.ROUTE_DATA_BREADCRUMB];
      if (label) {
        breadcrumbs.push({label, routerLink: url});
      }

      return this.createBreadcrumbs(child, url, breadcrumbs);
    }

    return breadcrumbs;
  }
}
