import { Component, OnInit, ViewChild } from '@angular/core';
import { NgxGalleryAnimation, NgxGalleryComponent, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  @ViewChild(NgxGalleryComponent) ngxGalleryComponent: NgxGalleryComponent;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  photos: Partial<Photo[]> = [];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.adminService.getPhotosToApprove().pipe(take(1)).subscribe(p => {
      this.photos = p;
      this.setImages();
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];
  }

  setImages() {
    const imageUrls = [];
    for (const photo of this.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      });
    }

    if (imageUrls.length == 0) {
      imageUrls.push({
        small: './assets/user.png',
        medium: './assets/user.png',
        big: './assets/user.png',
      })
    }

    this.galleryImages = imageUrls;
  }

  approvePhoto() {
    const p = this.getCurrentPhoto();
    this.adminService.approvePhoto(p.id).subscribe(this.GetOnCompleteFunc(p));
  }

  rejectPhoto() {
    const p = this.getCurrentPhoto();
    this.adminService.rejectPhoto(p.id).subscribe(this.GetOnCompleteFunc(p));
  }

  private GetOnCompleteFunc(p: Photo) {
    return () => {
      const index = this.photos.indexOf(p);
      if (this.photos.length > 1 && index > -1)
        this.photos.splice(index, 1);
      else
        this.photos = []
      this.setImages();
    }
  }

  getCurrentPhoto() {
    let current = this.galleryImages[this.ngxGalleryComponent.selectedIndex];
    return this.photos.find(p => p.url === current?.small)
  }
}
