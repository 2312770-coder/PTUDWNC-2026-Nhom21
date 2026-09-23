"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useState } from "react";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export default function RegisterPage() {
  const router = useRouter();

  const [form, setForm] = useState({
    email: "",
    displayName: "",
    userName: "",
    password: "",
    confirmPassword: "",
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const handleChange = (field: keyof typeof form, value: string) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setErrorMessage("");

    if (form.password.length < 8) {
      setErrorMessage("Mật khẩu phải có ít nhất 8 ký tự.");
      return;
    }

    if (form.password !== form.confirmPassword) {
      setErrorMessage("Mật khẩu xác nhận không khớp.");
      return;
    }

    setIsSubmitting(true);

    try {
      const response = await fetch(`${API_BASE_URL}/api/v1/auth/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: form.email.trim(),
          displayName: form.displayName.trim(),
          userName: form.userName.trim() || undefined,
          password: form.password,
        }),
      });

      const payload = await response.json().catch(() => null);

      if (!response.ok) {
        const message =
          payload?.message ||
          payload?.error ||
          payload?.title ||
          "Đăng ký thất bại. Vui lòng thử lại.";
        throw new Error(message);
      }

      const data = payload?.data ?? payload;
      const accessToken = data?.accessToken ?? data?.access_token;
      const refreshToken = data?.refreshToken ?? data?.refresh_token;

      if (accessToken) {
        localStorage.setItem("accessToken", accessToken);
      }

      if (refreshToken) {
        localStorage.setItem("refreshToken", refreshToken);
      }

      router.push("/");
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : "Đăng ký thất bại. Vui lòng thử lại."
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <main className="min-h-screen bg-gradient-to-br from-orange-50 via-white to-amber-50 px-4 py-12">
      <div className="mx-auto max-w-md rounded-3xl border border-orange-100 bg-white p-8 shadow-lg shadow-orange-100/60">
        <div className="mb-8 text-center">
          <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-2xl bg-orange-500 text-2xl text-white shadow-md shadow-orange-300">
            🍲
          </div>
          <h1 className="text-3xl font-bold text-slate-900">Đăng ký</h1>
          <p className="mt-2 text-sm text-slate-500">
            Tạo tài khoản để chia sẻ công thức và lưu trữ món ngon của bạn.
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5" noValidate>
          <div>
            <label htmlFor="email" className="mb-2 block text-sm font-medium text-slate-700">
              Email
            </label>
            <input
              id="email"
              type="email"
              required
              value={form.email}
              onChange={(e) => handleChange("email", e.target.value)}
              className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 outline-none transition focus:border-orange-400 focus:bg-white focus:ring-4 focus:ring-orange-100"
              placeholder="example@email.com"
            />
          </div>

          <div>
            <label htmlFor="displayName" className="mb-2 block text-sm font-medium text-slate-700">
              Tên hiển thị
            </label>
            <input
              id="displayName"
              type="text"
              required
              value={form.displayName}
              onChange={(e) => handleChange("displayName", e.target.value)}
              className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 outline-none transition focus:border-orange-400 focus:bg-white focus:ring-4 focus:ring-orange-100"
              placeholder="Nguyễn Văn A"
            />
          </div>

          <div>
            <label htmlFor="userName" className="mb-2 block text-sm font-medium text-slate-700">
              Tên đăng nhập (tùy chọn)
            </label>
            <input
              id="userName"
              type="text"
              value={form.userName}
              onChange={(e) => handleChange("userName", e.target.value)}
              className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 outline-none transition focus:border-orange-400 focus:bg-white focus:ring-4 focus:ring-orange-100"
              placeholder="nguyenvana"
            />
          </div>

          <div>
            <label htmlFor="password" className="mb-2 block text-sm font-medium text-slate-700">
              Mật khẩu
            </label>
            <input
              id="password"
              type="password"
              required
              value={form.password}
              onChange={(e) => handleChange("password", e.target.value)}
              className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 outline-none transition focus:border-orange-400 focus:bg-white focus:ring-4 focus:ring-orange-100"
              placeholder="Ít nhất 8 ký tự"
            />
          </div>

          <div>
            <label htmlFor="confirmPassword" className="mb-2 block text-sm font-medium text-slate-700">
              Xác nhận mật khẩu
            </label>
            <input
              id="confirmPassword"
              type="password"
              required
              value={form.confirmPassword}
              onChange={(e) => handleChange("confirmPassword", e.target.value)}
              className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 outline-none transition focus:border-orange-400 focus:bg-white focus:ring-4 focus:ring-orange-100"
              placeholder="Nhập lại mật khẩu"
            />
          </div>

          {errorMessage ? (
            <div className="rounded-xl border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-600">
              {errorMessage}
            </div>
          ) : null}

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-xl bg-orange-500 px-4 py-3 font-semibold text-white transition hover:bg-orange-600 disabled:cursor-not-allowed disabled:bg-orange-300"
          >
            {isSubmitting ? "Đang đăng ký..." : "Đăng ký"}
          </button>
        </form>

        <p className="mt-6 text-center text-sm text-slate-500">
          Đã có tài khoản?{" "}
          <Link href="/login" className="font-semibold text-orange-600 hover:text-orange-700">
            Đăng nhập
          </Link>
        </p>
      </div>
    </main>
  );
}
