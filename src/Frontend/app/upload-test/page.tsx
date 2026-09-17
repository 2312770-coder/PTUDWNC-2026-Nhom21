'use client';

import React, { useState } from 'react';
import ImageUploader from '@/components/ui/ImageUploader';
import Link from 'next/link';

export default function UploadPhotoPage() {
  const [uploadedImages, setUploadedImages] = useState<string[]>([]);

  const handleUploadSuccess = (url: string) => {
    setUploadedImages((prev) => [url, ...prev]);
  };

  return (
    <div className="min-h-screen bg-gradient-to-b from-orange-50/40 via-white to-neutral-50 py-12 px-4 sm:px-6">
      <div className="max-w-3xl mx-auto space-y-8">
        {/* Header */}
        <div className="text-center space-y-3">
          <Link
            href="/"
            className="inline-flex items-center gap-1.5 text-sm text-orange-600 hover:text-orange-700 font-medium transition"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
            </svg>
            Quay lại Trang chủ
          </Link>
          <h1 className="text-3xl sm:text-4xl font-extrabold text-neutral-900 tracking-tight">
            Thư Viện & Tải Ảnh Công Thức
          </h1>
          <p className="text-neutral-500 max-w-xl mx-auto text-sm leading-relaxed">
            Đăng tải hình ảnh món ăn sắc nét để làm phong phú thêm bài viết công thức và truyền cảm hứng cho cộng đồng người yêu ẩm thực.
          </p>
        </div>

        {/* Uploader Component */}
        <ImageUploader
          folder="recipes"
          onUploadSuccess={handleUploadSuccess}
        />

        {/* Danh sách ảnh đã upload */}
        {uploadedImages.length > 0 && (
          <div className="bg-white rounded-2xl p-6 border border-neutral-100 shadow-sm space-y-4">
            <h2 className="text-base font-bold text-neutral-900 flex items-center justify-between">
              <span>🖼️ Hình ảnh đã tải lên ({uploadedImages.length})</span>
              <span className="text-xs text-neutral-400 font-normal">Tự động tối ưu hóa hiển thị</span>
            </h2>
            <div className="grid grid-cols-2 sm:grid-cols-3 gap-4">
              {uploadedImages.map((url, idx) => (
                <div
                  key={idx}
                  className="group relative rounded-xl overflow-hidden border border-neutral-200/80 bg-neutral-50 aspect-square shadow-xs"
                >
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img
                    src={url}
                    alt={`Ảnh công thức ${idx + 1}`}
                    className="w-full h-full object-cover group-hover:scale-105 transition duration-300"
                  />
                  <div className="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition flex items-center justify-center p-2 text-center">
                    <a
                      href={url}
                      target="_blank"
                      rel="noreferrer"
                      className="text-xs text-white font-medium underline break-all line-clamp-2 hover:text-orange-200"
                    >
                      Xem ảnh gốc
                    </a>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Mẹo chụp ảnh cho người dùng */}
        <div className="p-5 bg-orange-50/60 border border-orange-200/60 rounded-2xl text-xs text-neutral-700 space-y-2">
          <p className="font-bold text-orange-900 flex items-center gap-1.5 text-sm">
            <span>✨</span> Mẹo chụp ảnh món ăn cuốn hút:
          </p>
          <ul className="list-disc list-inside space-y-1.5 text-neutral-600 pl-1 leading-relaxed">
            <li>Ưu tiên ánh sáng tự nhiên vào ban ngày để màu sắc nguyên liệu tươi tắn, chân thực nhất.</li>
            <li>Chụp cận cảnh (close-up) chi tiết từng thớ thịt, lớp nước sốt óng ánh để kích thích vị giác.</li>
            <li>Hỗ trợ các định dạng phổ biến: JPG, PNG, WebP với dung lượng tối đa 5MB mỗi ảnh.</li>
          </ul>
        </div>
      </div>
    </div>
  );
}
