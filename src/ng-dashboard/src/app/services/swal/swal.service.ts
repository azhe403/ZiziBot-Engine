import {Injectable} from '@angular/core';
import Swal, {SweetAlertIcon, SweetAlertResult} from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class SwalService {

  constructor() {
  }

  public showNotification(title: string, text: string, icon: SweetAlertIcon): Promise<SweetAlertResult<Awaited<any>>> {
    return Swal.fire({
      position: 'top-end',
      icon: icon,
      title: title,
      text: text,
      showConfirmButton: false,
      timer: 3000
    })
  }

  public showConfirmation(title: string, text: string, icon: SweetAlertIcon): Promise<SweetAlertResult<Awaited<any>>> {
    return Swal.fire({
      title: title,
      text: text,
      icon: icon,
      showCancelButton: true,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    })
  }
}