"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useState } from "react";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export default function LoginPage() {
  const router = useRouter();

  const [form, setForm] = useState({
    email: "",
    password: "",
  });
  const [showPassword, setShowPassword] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [isLocked, setIsLocked] = useState(false);

  const handleChange = (field: keyof typeof form, value: string) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setErrorMessage("");
    setIsLocked(false);

    if (!form.email.trim()) {
      setErrorMessage("Vui lòng nhập địa chỉ email.");
      return;
    }

    if (!form.password) {
      setErrorMessage("Vui lòng nhập mật khẩu.");
      return;
    }

    setIsSubmitting(true);

    try {
      const response = await fetch(`${API_BASE_URL}/api/v1/auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: form.email.trim(),
          password: form.password,
        }),
      });

      const payload = await response.json().catch(() => null);

      if (!response.ok) {
        if (response.status === 423) {
          setIsLocked(true);
          throw new Error(
            payload?.detail ||
              "Tài khoản đã bị tạm khóa do nhập sai mật khẩu quá 5 lần. Vui lòng thử lại sau 15 phút."
          );
        }

        if (response.status === 429) {
          throw new Error(
            payload?.detail ||
              "Bạn đã gửi quá nhiều yêu cầu đăng nhập. Vui lòng đợi 1 phút rồi thử lại."
          );
        }

        if (response.status === 401) {
          throw new Error(
            payload?.detail || "Email hoặc mật khẩu không chính xác."
          );
        }

        const message =
          payload?.detail ||
          payload?.message ||
          payload?.title ||
          "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.";
        throw new Error(message);
      }

      const data = payload?.data ?? payload;
      const accessToken = data?.accessToken ?? data?.access_token;
      const refreshToken = data?.refreshToken ?? data?.refresh_token;
      const user = data?.user;

      if (accessToken) {
        localStorage.setItem("accessToken", accessToken);
      }

      if (refreshToken) {
        localStorage.setItem("refreshToken", refreshToken);
      }

      if (user) {
        localStorage.setItem("user", JSON.stringify(user));
      }

      // Thông báo cho Navbar và các component khác cập nhật trạng thái
      if (typeof window !== "undefined") {
        window.dispatchEvent(new Event("auth-changed"));
      }

      router.push("/");
      router.refresh();
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : "Đăng nhập thất bại. Vui lòng thử lại."
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
          <h1 className="text-3xl font-bold text-slate-900">Đăng nhập</h1>
          <p className="mt-2 text-sm text-slate-500">
            Chào mừng bạn quay trở lại với cộng đồng ẩm thực Culinary Blog.
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
              autoComplete="email"
              value={form.email}
              onChange={(e) => handleChange("email", e.target.value)}
              className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 outline-none transition focus:border-orange-400 focus:bg-white focus:ring-4 focus:ring-orange-100"
              placeholder="example@email.com"
            />
          </div>

          <div>
            <div className="mb-2 flex items-center justify-between">
              <label htmlFor="password" className="block text-sm font-medium text-slate-700">
                Mật khẩu
              </label>
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="text-xs font-medium text-orange-600 hover:text-orange-700"
              >
                {showPassword ? "Ẩn" : "Hiện"}
              </button>
            </div>
            <div className="relative">
              <input
                id="password"
                type={showPassword ? "text" : "password"}
                required
                autoComplete="current-password"
                value={form.password}
                onChange={(e) => handleChange("password", e.target.value)}
                className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 outline-none transition focus:border-orange-400 focus:bg-white focus:ring-4 focus:ring-orange-100"
                placeholder="Nhập mật khẩu"
              />
            </div>
          </div>

          {errorMessage ? (
            <div
              className={`rounded-xl border px-3.5 py-2.5 text-sm ${
                isLocked
                  ? "border-red-300 bg-red-100/80 font-medium text-red-800"
                  : "border-red-200 bg-red-50 text-red-600"
              }`}
            >
              <div className="flex items-start gap-2">
                <span className="text-base">{isLocked ? "🔒" : "⚠️"}</span>
                <span>{errorMessage}</span>
              </div>
            </div>
          ) : null}

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-xl bg-orange-500 px-4 py-3 font-semibold text-white shadow-md shadow-orange-500/20 transition hover:bg-orange-600 disabled:cursor-not-allowed disabled:bg-orange-300"
          >
            {isSubmitting ? "Đang đăng nhập..." : "Đăng nhập"}
          </button>
        </form>

        <p className="mt-6 text-center text-sm text-slate-500">
          Chưa có tài khoản?{" "}
          <Link href="/register" className="font-semibold text-orange-600 hover:text-orange-700">
            Đăng ký ngay
          </Link>
        </p>
      </div>
    </main>
  );
}
