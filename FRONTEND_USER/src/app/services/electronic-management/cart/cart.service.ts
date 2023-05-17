import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { DA_SERVICE_TOKEN, ITokenService } from '@delon/auth';

import { environment } from '@env/environment';

import { cartRouter } from '@util';
import { NzMessageService } from 'ng-zorro-antd/message';
// RxJS
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private http: HttpClient,
    @Inject(DA_SERVICE_TOKEN) private tokenService: ITokenService,
    private router: Router,
    private nzMessage: NzMessageService,) {}
  private listCartSource = new BehaviorSubject([]);
  currentCart = this.listCartSource.asObservable();
  changeCart(listCart: any) {
    this.listCartSource.next(listCart);
  }
update(model: any): Observable<any> {
    return this.http.put(environment.BASE_API_URL + cartRouter.update, model);
  }

  getById(): Observable<any> {
    return this.http.get(environment.BASE_API_URL + cartRouter.getById);
  }

  delete(list: [string]): Observable<any> {
    const option = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body: list,
    };
    return this.http.request('delete', environment.BASE_API_URL + cartRouter.delete, option);
  }
  removeItem(item: any, listCart: any[]) {
    const index = listCart.indexOf(item);
    listCart.splice(index, 1);
    // localStorage.setItem('list-cart', JSON.stringify(listCart));
    this.changeCart(listCart);
    const cartModel = {
      listProducts: JSON.stringify(listCart),
    };
    this.changeCart(listCart);
    this.update(cartModel).subscribe((res) => {
      if (res.code === 200) {
        this.nzMessage.success('Update cart success');
      } else {
        this.nzMessage.success('CHave some error');
      }
    });
    return listCart;
  }
  change(event: any, prod: any, listCart: any[]) {
    let total = 0;
    listCart.map((item) => {
      if (item.id === prod.id) {
        item.count = event;
      }
    });
    listCart.map((item) => {
      item.subTotal = item.count * (item.amoutDefault - item.discountDefault);
      total = total + item.subTotal;
    });
    // localStorage.setItem('list-cart', JSON.stringify(listCart));
    this.changeCart(listCart);
    const cartModel = {
      listProducts: JSON.stringify(listCart),
    };
    this.update(cartModel).subscribe((res) => {
      if (res.code === 200) {
        this.nzMessage.success('Update cart success');
      } else {
        this.nzMessage.success('Have some error');
      }
    });
    const modelReturn = {
      total: total,
      listCart: listCart,
    };
    return modelReturn;
  }
  addToCart(item: any, url: any) {
    const token = this.tokenService.get()?.token;
    console.log(token);
    
    if (token) {
      this.getById().subscribe((res) => {
        if (res.code === 200) {
          const listCart = res.data !== null ? res.data : [];
          if (listCart.length > 0) {
            let flag = 0;
            listCart.forEach((c: any) => {
              if (c.id === item.id && c.productColors[0].name === item.productColors[0].name) {
                c.count++;
                flag = 1;
              }
            });
            if (flag === 0) {
              //item.count = 1;
              listCart.push(item);
            }
          } else {
            //item.count = 1;
            listCart.push(item);
          }
          const cartModel = {
            listProducts: JSON.stringify(listCart),
          };
          this.update(cartModel).subscribe((res) => {
            if (res.code === 200) {
              this.nzMessage.success('Add to cart success');
            } else {
              this.nzMessage.success('Have some error');
            }
          });
          this.changeCart(listCart);
        }
      });
    } else {
      this.nzMessage.error('Login to buy this product, please');
      this.router.navigate(['/login']);
    }
  }
}
