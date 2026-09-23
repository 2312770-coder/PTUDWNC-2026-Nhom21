// ============================================================================
// COMPONENT: StepListEditor.tsx
// CHỨC NĂNG: FR-RCP-010 - Quản lý các bước nấu ăn (Step List Editor)
// THÀNH VIÊN: Nguyễn Đình Tuấn (MSSV: 2312792)
// QUYẾT ĐỊNH KIẾN TRÚC TUÂN THỦ: D9 (Tự động sinh stepNumber, tiêu đề bắt buộc)
// ============================================================================

'use client';

import React, { useState } from 'react';
import Image from 'next/image';
import ImageUploader from '@/components/ui/ImageUploader';
import { recipesApi } from '@/lib/api/recipes';
import type { RecipeStepDto } from '@/types/api';

/**
 * Props nhận vào cho Component StepListEditor
 */
interface StepListEditorProps {
  recipeId: string; // ID của công thức đang chỉnh sửa
  initialSteps?: RecipeStepDto[]; // Danh sách bước ban đầu (nếu có)
  onStepsChange?: (steps: RecipeStepDto[]) => void; // Callback thông báo khi danh sách bước thay đổi
}

/**
 * Component quản lý toàn diện các bước thực hiện công thức nấu ăn (FR-RCP-010).
 * Cho phép: Xem danh sách đánh số, thêm bước mới, sửa bước, xóa bước và tải ảnh minh họa MinIO.
 */
export default function StepListEditor({
  recipeId,
  initialSteps = [],
  onStepsChange,
}: StepListEditorProps) {
  // 1. Quản lý danh sách các bước nấu trong State
  const [steps, setSteps] = useState<RecipeStepDto[]>(initialSteps);

  // 2. Trạng thái Form thêm bước mới
  const [isAdding, setIsAdding] = useState(false);
  const [newTitle, setNewTitle] = useState('');
  const [newDescription, setNewDescription] = useState('');
  const [newTimerMinutes, setNewTimerMinutes] = useState<number | ''>('');
  const [newImageUrl, setNewImageUrl] = useState('');

  // 3. Trạng thái chỉnh sửa bước đang chọn
  const [editingStepId, setEditingStepId] = useState<string | null>(null);
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');
  const [editTimerMinutes, setEditTimerMinutes] = useState<number | ''>('');
  const [editImageUrl, setEditImageUrl] = useState('');

  // 4. Trạng thái xử lý mạng và thông báo lỗi/thành công
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  /**
   * Helper hiển thị thông báo thành công trong 3 giây
   */
  const showToast = (msg: string) => {
    setSuccessMessage(msg);
    setTimeout(() => setSuccessMessage(null), 3500);
  };

  /**
   * Xử lý thêm bước mới vào công thức (POST /api/v1/recipes/{id}/steps)
   * Tuân thủ D9: Không truyền StepNumber, Server tự động gán Max + 1
   */
  const handleAddStep = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newTitle.trim()) {
      setErrorMessage('Vui lòng nhập tiêu đề cho bước nấu.');
      return;
    }
    if (!newDescription.trim()) {
      setErrorMessage('Vui lòng nhập nội dung hướng dẫn chi tiết.');
      return;
    }

    setIsLoading(true);
    setErrorMessage(null);

    try {
      // Gọi API thêm bước mới (D9: StepNumber tự sinh trên server)
      const addedStep = await recipesApi.addStep(recipeId, {
        title: newTitle.trim(),
        description: newDescription.trim(),
        timerMinutes: typeof newTimerMinutes === 'number' ? newTimerMinutes : undefined,
        imageUrl: newImageUrl.trim() || undefined,
      });

      // Cập nhật state danh sách bước mới
      const updated = [...steps, addedStep].sort((a, b) => a.stepNumber - b.stepNumber);
      setSteps(updated);
      onStepsChange?.(updated);

      // Đặt lại dữ liệu form thêm mới
      setNewTitle('');
      setNewDescription('');
      setNewTimerMinutes('');
      setNewImageUrl('');
      setIsAdding(false);
      showToast('Đã thêm bước nấu mới thành công!');
    } catch (err: unknown) {
      const errObj = err as { response?: { data?: { detail?: string; title?: string } }; message?: string };
      setErrorMessage(errObj.response?.data?.detail || errObj.response?.data?.title || errObj.message || 'Thêm bước nấu thất bại.');
    } finally {
      setIsLoading(false);
    }
  };

  /**
   * Mở form chỉnh sửa cho một bước cụ thể
   */
  const handleStartEdit = (step: RecipeStepDto) => {
    setEditingStepId(step.id);
    setEditTitle(step.title);
    setEditDescription(step.description);
    setEditTimerMinutes(step.timerMinutes ?? '');
    setEditImageUrl(step.imageUrl ?? '');
    setErrorMessage(null);
  };

  /**
   * Hủy bỏ chế độ chỉnh sửa
   */
  const handleCancelEdit = () => {
    setEditingStepId(null);
    setEditTitle('');
    setEditDescription('');
    setEditTimerMinutes('');
    setEditImageUrl('');
  };

  /**
   * Xử lý lưu thông tin bước đã chỉnh sửa (PUT /api/v1/recipes/{id}/steps/{stepId})
   */
  const handleSaveEdit = async (stepId: string) => {
    if (!editTitle.trim()) {
      setErrorMessage('Tiêu đề bước nấu không được để trống.');
      return;
    }
    if (!editDescription.trim()) {
      setErrorMessage('Mô tả bước nấu không được để trống.');
      return;
    }

    setIsLoading(true);
    setErrorMessage(null);

    try {
      // Gửi yêu cầu cập nhật lên Backend
      const updatedStep = await recipesApi.updateStep(recipeId, stepId, {
        title: editTitle.trim(),
        description: editDescription.trim(),
        timerMinutes: typeof editTimerMinutes === 'number' ? editTimerMinutes : undefined,
        imageUrl: editImageUrl.trim() || undefined,
      });

      // Cập nhật lại danh sách trên giao diện
      const updated = steps.map((s) => (s.id === stepId ? updatedStep : s));
      setSteps(updated);
      onStepsChange?.(updated);

      handleCancelEdit();
      showToast('Đã lưu thay đổi bước nấu!');
    } catch (err: unknown) {
      const errObj = err as { response?: { data?: { detail?: string; title?: string } }; message?: string };
      setErrorMessage(errObj.response?.data?.detail || errObj.response?.data?.title || errObj.message || 'Lỗi khi cập nhật bước.');
    } finally {
      setIsLoading(false);
    }
  };

  /**
   * Xử lý xóa bước nấu (DELETE /api/v1/recipes/{id}/steps/{stepId})
   * Backend sẽ tự động renumber lại các bước còn lại (1, 2, 3...)
   */
  const handleDeleteStep = async (stepId: string, stepNumber: number) => {
    const confirm = window.confirm(`Bạn có chắc chắn muốn xóa Bước ${stepNumber} không?`);
    if (!confirm) return;

    setIsLoading(true);
    setErrorMessage(null);

    try {
      await recipesApi.deleteStep(recipeId, stepId);

      // Tải lại danh sách bước mới nhất từ server để đảm bảo đúng số thứ tự đã renumber
      const refreshedSteps = await recipesApi.getSteps(recipeId);
      setSteps(refreshedSteps);
      onStepsChange?.(refreshedSteps);

      showToast(`Đã xóa Bước ${stepNumber} thành công!`);
    } catch (err: unknown) {
      const errObj = err as { response?: { data?: { detail?: string; title?: string } }; message?: string };
      setErrorMessage(errObj.response?.data?.detail || errObj.message || 'Xóa bước nấu thất bại.');
    } finally {
      setIsLoading(false);
    }
  };

  /**
   * Xử lý di chuyển thứ tự bước (Đổi chỗ với bước phía trên hoặc phía dưới)
   */
  const handleMoveStep = async (step: RecipeStepDto, direction: 'up' | 'down') => {
    const currentIndex = steps.findIndex((s) => s.id === step.id);
    if (currentIndex === -1) return;

    const targetIndex = direction === 'up' ? currentIndex - 1 : currentIndex + 1;
    if (targetIndex < 0 || targetIndex >= steps.length) return;

    const targetStepNumber = targetIndex + 1;

    setIsLoading(true);
    setErrorMessage(null);

    try {
      // Gửi PUT với StepNumber mới để Backend reorder toàn bộ danh sách
      await recipesApi.updateStep(recipeId, step.id, {
        title: step.title,
        description: step.description,
        stepNumber: targetStepNumber,
        timerMinutes: step.timerMinutes,
        imageUrl: step.imageUrl,
      });

      // Tải lại danh sách đã sắp xếp từ server
      const refreshed = await recipesApi.getSteps(recipeId);
      setSteps(refreshed);
      onStepsChange?.(refreshed);
      showToast('Đã cập nhật lại thứ tự các bước!');
    } catch (err: unknown) {
      const errObj = err as { response?: { data?: { detail?: string; title?: string } }; message?: string };
      setErrorMessage(errObj.response?.data?.detail || errObj.message || 'Không thể đổi thứ tự bước.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="bg-white rounded-2xl border border-neutral-200 p-6 shadow-sm">
      {/* Header của khối quản lý bước nấu */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 pb-5 border-b border-neutral-100">
        <div>
          <h2 className="text-xl font-bold text-neutral-900 flex items-center gap-2">
            <span>📋</span> Các bước thực hiện
          </h2>
          <p className="text-sm text-neutral-500 mt-1">
            Tổng cộng: <strong className="text-neutral-800">{steps.length} bước</strong>. Sắp xếp theo thứ tự thực hiện từ đầu đến cuối.
          </p>
        </div>

        {/* Nút bấm bật mở Form thêm bước mới */}
        {!isAdding && (
          <button
            type="button"
            onClick={() => setIsAdding(true)}
            className="inline-flex items-center gap-2 px-4 py-2.5 bg-orange-600 hover:bg-orange-700 text-white font-medium rounded-xl transition-colors shadow-sm"
          >
            <span>➕</span> Thêm bước mới
          </button>
        )}
      </div>

      {/* Thông báo lỗi nếu có */}
      {errorMessage && (
        <div className="mt-4 p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm flex items-start justify-between">
          <div className="flex items-center gap-2">
            <span>⚠️</span>
            <span>{errorMessage}</span>
          </div>
          <button onClick={() => setErrorMessage(null)} className="text-red-500 hover:text-red-700 text-xs font-bold">
            ✕
          </button>
        </div>
      )}

      {/* Thông báo thành công nếu có */}
      {successMessage && (
        <div className="mt-4 p-3 bg-green-50 border border-green-200 rounded-xl text-green-700 text-sm flex items-center gap-2">
          <span>✅</span>
          <span>{successMessage}</span>
        </div>
      )}

      {/* Form thêm bước mới */}
      {isAdding && (
        <form onSubmit={handleAddStep} className="mt-6 p-5 bg-orange-50/60 border border-orange-200 rounded-2xl space-y-4">
          <div className="flex items-center justify-between">
            <h3 className="font-bold text-neutral-900 text-base flex items-center gap-2">
              <span className="w-7 h-7 bg-orange-600 text-white rounded-full flex items-center justify-center text-xs">
                {steps.length + 1}
              </span>
              Thêm bước nấu tiếp theo
            </h3>
            <button
              type="button"
              onClick={() => setIsAdding(false)}
              className="text-neutral-400 hover:text-neutral-600 text-sm"
            >
              ✕ Hủy
            </button>
          </div>

          <div>
            <label className="block text-xs font-semibold text-neutral-700 uppercase tracking-wider mb-1.5">
              Tiêu đề bước <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              required
              value={newTitle}
              onChange={(e) => setNewTitle(e.target.value)}
              placeholder="Ví dụ: Sơ chế và ướp thịt bò, Đun nước dùng..."
              className="w-full px-3.5 py-2.5 bg-white border border-neutral-300 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-neutral-700 uppercase tracking-wider mb-1.5">
              Hướng dẫn chi tiết <span className="text-red-500">*</span>
            </label>
            <textarea
              required
              rows={3}
              value={newDescription}
              onChange={(e) => setNewDescription(e.target.value)}
              placeholder="Mô tả cụ thể thao tác, mẹo nấu hoặc dấu hiệu món ăn đạt yêu cầu..."
              className="w-full px-3.5 py-2.5 bg-white border border-neutral-300 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-semibold text-neutral-700 uppercase tracking-wider mb-1.5">
                ⏱️ Thời gian đếm ngược (phút - tùy chọn)
              </label>
              <input
                type="number"
                min="0"
                value={newTimerMinutes}
                onChange={(e) => setNewTimerMinutes(e.target.value === '' ? '' : Math.max(0, parseInt(e.target.value, 10)))}
                placeholder="Ví dụ: 15"
                className="w-full px-3.5 py-2.5 bg-white border border-neutral-300 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
            </div>

            <div>
              <label className="block text-xs font-semibold text-neutral-700 uppercase tracking-wider mb-1.5">
                🖼️ Tải ảnh minh họa (tùy chọn)
              </label>
              <ImageUploader
                folder={`recipes/${recipeId}/steps`}
                onUploadSuccess={(url) => setNewImageUrl(url)}
                initialUrl={newImageUrl}
              />
            </div>
          </div>

          <div className="flex items-center justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => setIsAdding(false)}
              className="px-4 py-2 border border-neutral-300 text-neutral-700 rounded-xl text-sm font-medium hover:bg-white"
            >
              Hủy
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="px-5 py-2 bg-orange-600 hover:bg-orange-700 text-white rounded-xl text-sm font-semibold shadow-sm disabled:opacity-50"
            >
              {isLoading ? 'Đang lưu...' : 'Lưu bước này'}
            </button>
          </div>
        </form>
      )}

      {/* Danh sách các bước nấu */}
      <div className="mt-6 space-y-4">
        {steps.length === 0 ? (
          <div className="text-center py-12 border-2 border-dashed border-neutral-200 rounded-2xl">
            <span className="text-4xl block mb-2">🍳</span>
            <p className="text-neutral-500 font-medium">Chưa có bước nấu nào cho công thức này.</p>
            <p className="text-xs text-neutral-400 mt-1">Bấm nút &quot;Thêm bước mới&quot; ở trên để bắt đầu nhập liệu.</p>
          </div>
        ) : (
          steps.map((step, index) => {
            const isEditingThis = editingStepId === step.id;

            return (
              <div
                key={step.id}
                className="border border-neutral-200 rounded-2xl p-5 bg-white hover:border-orange-200 transition-colors"
              >
                {/* Chế độ chỉnh sửa */}
                {isEditingThis ? (
                  <div className="space-y-4">
                    <div className="flex items-center justify-between border-b pb-2">
                      <span className="font-bold text-orange-600 text-sm">Chỉnh sửa Bước {step.stepNumber}</span>
                      <button onClick={handleCancelEdit} className="text-neutral-400 hover:text-neutral-600 text-xs">
                        ✕ Hủy
                      </button>
                    </div>

                    <div>
                      <label className="block text-xs font-semibold text-neutral-600 mb-1">Tiêu đề bước</label>
                      <input
                        type="text"
                        value={editTitle}
                        onChange={(e) => setEditTitle(e.target.value)}
                        className="w-full px-3 py-2 border border-neutral-300 rounded-lg text-sm focus:ring-2 focus:ring-orange-500"
                      />
                    </div>

                    <div>
                      <label className="block text-xs font-semibold text-neutral-600 mb-1">Mô tả chi tiết</label>
                      <textarea
                        rows={3}
                        value={editDescription}
                        onChange={(e) => setEditDescription(e.target.value)}
                        className="w-full px-3 py-2 border border-neutral-300 rounded-lg text-sm focus:ring-2 focus:ring-orange-500"
                      />
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-xs font-semibold text-neutral-600 mb-1">Hẹn giờ (phút)</label>
                        <input
                          type="number"
                          min="0"
                          value={editTimerMinutes}
                          onChange={(e) => setEditTimerMinutes(e.target.value === '' ? '' : Math.max(0, parseInt(e.target.value, 10)))}
                          className="w-full px-3 py-2 border border-neutral-300 rounded-lg text-sm"
                        />
                      </div>
                      <div>
                        <label className="block text-xs font-semibold text-neutral-600 mb-1">Ảnh minh họa</label>
                        <ImageUploader
                          folder={`recipes/${recipeId}/steps`}
                          onUploadSuccess={(url) => setEditImageUrl(url)}
                          initialUrl={editImageUrl}
                        />
                      </div>
                    </div>

                    <div className="flex justify-end gap-2 pt-2">
                      <button
                        type="button"
                        onClick={handleCancelEdit}
                        className="px-3 py-1.5 text-sm text-neutral-600 border rounded-lg hover:bg-neutral-50"
                      >
                        Hủy
                      </button>
                      <button
                        type="button"
                        onClick={() => handleSaveEdit(step.id)}
                        disabled={isLoading}
                        className="px-4 py-1.5 text-sm bg-orange-600 text-white font-medium rounded-lg hover:bg-orange-700 disabled:opacity-50"
                      >
                        {isLoading ? 'Đang lưu...' : 'Lưu cập nhật'}
                      </button>
                    </div>
                  </div>
                ) : (
                  /* Chế độ hiển thị bình thường */
                  <div className="flex flex-col sm:flex-row gap-4 justify-between items-start">
                    <div className="flex gap-4 items-start flex-1">
                      {/* Số thứ tự bước */}
                      <div className="w-10 h-10 rounded-xl bg-neutral-900 text-white flex-shrink-0 flex items-center justify-center font-bold text-base shadow-sm">
                        {step.stepNumber}
                      </div>

                      {/* Nội dung tiêu đề, mô tả và hẹn giờ */}
                      <div className="space-y-1.5 flex-1">
                        <div className="flex flex-wrap items-center gap-2">
                          <h3 className="font-bold text-neutral-900 text-base">{step.title}</h3>
                          {step.timerMinutes && step.timerMinutes > 0 && (
                            <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-amber-100 text-amber-800">
                              ⏱️ {step.timerMinutes} phút
                            </span>
                          )}
                        </div>
                        <p className="text-sm text-neutral-600 whitespace-pre-line leading-relaxed">
                          {step.description}
                        </p>

                        {/* Ảnh minh họa của bước nếu có */}
                        {step.imageUrl && (
                          <div className="mt-3 relative w-36 h-24 rounded-lg overflow-hidden border border-neutral-200">
                            <Image
                              src={step.imageUrl}
                              alt={step.title}
                              fill
                              className="object-cover"
                              unoptimized
                            />
                          </div>
                        )}
                      </div>
                    </div>

                    {/* Các nút thao tác (Lên / Xuống / Sửa / Xóa) */}
                    <div className="flex items-center gap-1 self-end sm:self-start bg-neutral-50 p-1.5 rounded-xl border border-neutral-200">
                      {/* Nút di chuyển lên */}
                      <button
                        type="button"
                        title="Di chuyển lên"
                        disabled={index === 0 || isLoading}
                        onClick={() => handleMoveStep(step, 'up')}
                        className="w-8 h-8 flex items-center justify-center rounded-lg text-neutral-600 hover:bg-white hover:shadow-xs disabled:opacity-30 disabled:hover:bg-transparent"
                      >
                        ▲
                      </button>

                      {/* Nút di chuyển xuống */}
                      <button
                        type="button"
                        title="Di chuyển xuống"
                        disabled={index === steps.length - 1 || isLoading}
                        onClick={() => handleMoveStep(step, 'down')}
                        className="w-8 h-8 flex items-center justify-center rounded-lg text-neutral-600 hover:bg-white hover:shadow-xs disabled:opacity-30 disabled:hover:bg-transparent"
                      >
                        ▼
                      </button>

                      <div className="w-px h-5 bg-neutral-300 mx-0.5" />

                      {/* Nút Sửa bước */}
                      <button
                        type="button"
                        title="Sửa bước này"
                        onClick={() => handleStartEdit(step)}
                        className="px-2.5 py-1 text-xs font-semibold text-neutral-700 hover:bg-white rounded-lg hover:shadow-xs"
                      >
                        ✏️ Sửa
                      </button>

                      {/* Nút Xóa bước */}
                      <button
                        type="button"
                        title="Xóa bước này"
                        onClick={() => handleDeleteStep(step.id, step.stepNumber)}
                        className="px-2.5 py-1 text-xs font-semibold text-red-600 hover:bg-red-50 rounded-lg"
                      >
                        🗑️ Xóa
                      </button>
                    </div>
                  </div>
                )}
              </div>
            );
          })
        )}
      </div>
    </div>
  );
}
