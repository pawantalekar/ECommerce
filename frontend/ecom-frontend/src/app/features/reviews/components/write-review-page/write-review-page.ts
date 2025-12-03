import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReviewService } from '../../services/review-service';
import { Review } from '../../models/review.model';
import { CreateReviewRequest } from '../../models/create-review-request.model';
import { UpdateReviewRequest } from '../../models/update-review-request.model';

@Component({
  selector: 'app-write-review-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './write-review-page.html'
})
export class WriteReviewPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private reviewService = inject(ReviewService);

  productId = history.state.productId as string;
  review: Review | null = null;
  isEdit = false;

  form = this.fb.group({
    rating: [0, [Validators.required, Validators.min(1), Validators.max(5)]],
    comment: ['']
  });

  ngOnInit(): void {
    // If no productId from state → redirect (safety)
    if (!this.productId) {
      alert('No product selected for review');
      this.router.navigate(['/']);
      return;
    }

    // Load existing review (if any)
    this.reviewService.getMyReview(this.productId).subscribe({
      next: (r) => {
        if (r) {
          this.review = r;
          this.isEdit = true;
          this.form.patchValue({
            rating: r.rating,
            comment: r.comment || ''
          });
        }
      }
    });
  }

  submit() {
    if (this.form.invalid || !this.productId) return;

    const dto: CreateReviewRequest = {
      productId: this.productId,
      rating: this.form.value.rating!,
      comment: this.form.value.comment || null
    };

    const action = this.isEdit
      ? this.reviewService.updateReview(this.review!.id, dto as UpdateReviewRequest)
      : this.reviewService.createReview(dto);

    action.subscribe({
      next: () => this.router.navigate(['/']),
      error: (err) => alert('Failed to save review: ' + (err.error?.title || err.message))
    });
  }

  delete() {
    if (!this.review || !confirm('Delete your review?')) return;
    this.reviewService.deleteReview(this.review.id).subscribe({
      next: () => this.router.navigate(['/'])
    });
  }

  cancel() {
    window.history.back();
  }

  get stars(): number[] {
    return [1, 2, 3, 4, 5];
  }
}