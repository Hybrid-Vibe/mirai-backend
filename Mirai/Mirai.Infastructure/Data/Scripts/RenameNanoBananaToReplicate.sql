-- Migrate ai_images from NanoBanana to Replicate tracking column.
-- Run once against PostgreSQL before deploying the Replicate integration.

ALTER TABLE ai_images
    RENAME COLUMN nano_banana_request_id TO replicate_prediction_id;

-- If the old column never existed (fresh install), use instead:
-- ALTER TABLE ai_images ADD COLUMN IF NOT EXISTS replicate_prediction_id VARCHAR(255);
