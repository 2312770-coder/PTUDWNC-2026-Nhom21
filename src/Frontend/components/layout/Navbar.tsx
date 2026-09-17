"use client";

import React, { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function Navbar() {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const router = useRouter();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      router.push(`/recipes?q=${encodeURIComponent(searchQuery.trim())}`);
    }
  };

  return (
    <header className="sticky top-0 z-50 w-full border-b border-orange-100 bg-white/95 backdrop-blur-md transition-all shadow-sm">
      <div className="container mx-auto flex h-20 items-center justify-between px-4 sm:px-6 lg:px-8">
        {/* Logo */}
        <Link href="/" className="flex items-center gap-3 group">
          <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-gradient-to-tr from-amber-500 to-orange-500 text-white shadow-md shadow-orange-500/20 group-hover:scale-105 transition-transform">
            <svg
              className="h-6 w-6"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"
              />
            </svg>
          </div>
          <div>
            <span className="text-xl font-bold tracking-tight text-neutral-900">
              Culinary<span className="text-orange-600">Blog</span>
            </span>
            <span className="block text-[10px] uppercase tracking-wider font-semibold text-neutral-400">
              Cộng Đồng Ẩm Thực Việt
            </span>
          </div>
        </Link>

        {/* Navigation Links - Desktop */}
        <nav className="hidden md:flex items-center gap-8 text-sm font-medium text-neutral-600">
          <Link href="/" className="hover:text-orange-600 transition-colors">
            Trang chủ
          </Link>
          <Link href="/recipes" className="hover:text-orange-600 transition-colors">
            Khám phá công thức
          </Link>
          <a href="#categories" className="hover:text-orange-600 transition-colors">
            Danh mục
          </a>
          <a href="#about" className="hover:text-orange-600 transition-colors">
            Về chúng tôi
          </a>
        </nav>

        {/* Search Bar - Desktop */}
        <form onSubmit={handleSearch} className="hidden lg:flex items-center relative w-64">
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Tìm món ăn, nguyên liệu..."
            className="w-full rounded-full border border-neutral-200 bg-neutral-50 py-2 pl-4 pr-10 text-sm outline-none transition-all focus:border-orange-500 focus:bg-white focus:ring-2 focus:ring-orange-100"
          />
          <button
            type="submit"
            aria-label="Tìm kiếm"
            className="absolute right-3 text-neutral-400 hover:text-orange-600 transition-colors"
          >
            <svg
              className="h-4 w-4"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
              />
            </svg>
          </button>
        </form>

        {/* Action Buttons - Desktop */}
        <div className="hidden sm:flex items-center gap-3">
          <Link
            href="/recipes/create"
            className="inline-flex items-center gap-1.5 rounded-full bg-orange-50 px-4 py-2 text-xs font-semibold text-orange-700 hover:bg-orange-100 transition-colors border border-orange-200/60"
          >
            <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Viết công thức
          </Link>
          <Link
            href="/login"
            className="rounded-full px-4 py-2 text-xs font-semibold text-neutral-700 hover:text-orange-600 transition-colors"
          >
            Đăng nhập
          </Link>
          <Link
            href="/register"
            className="rounded-full bg-orange-600 px-4 py-2 text-xs font-semibold text-white shadow-sm hover:bg-orange-700 transition-colors"
          >
            Đăng ký
          </Link>
        </div>

        {/* Mobile menu button */}
        <button
          onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
          type="button"
          aria-label="Menu"
          className="flex md:hidden p-2 rounded-lg text-neutral-600 hover:bg-neutral-100 transition-colors"
        >
          <svg className="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            {isMobileMenuOpen ? (
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            ) : (
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
            )}
          </svg>
        </button>
      </div>

      {/* Mobile Menu Dropdown */}
      {isMobileMenuOpen && (
        <div className="md:hidden border-t border-neutral-100 bg-white px-4 pt-3 pb-6 space-y-4 shadow-lg animate-in slide-in-from-top duration-200">
          <form onSubmit={handleSearch} className="relative w-full">
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Tìm món ăn, nguyên liệu..."
              className="w-full rounded-lg border border-neutral-200 bg-neutral-50 py-2.5 pl-4 pr-10 text-sm outline-none focus:border-orange-500 focus:bg-white"
            />
            <button type="submit" className="absolute right-3 top-3 text-neutral-400">
              <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </button>
          </form>

          <nav className="flex flex-col space-y-3 font-medium text-neutral-700">
            <Link
              href="/"
              onClick={() => setIsMobileMenuOpen(false)}
              className="px-2 py-1.5 rounded-md hover:bg-orange-50 hover:text-orange-600 transition-colors"
            >
              Trang chủ
            </Link>
            <Link
              href="/recipes"
              onClick={() => setIsMobileMenuOpen(false)}
              className="px-2 py-1.5 rounded-md hover:bg-orange-50 hover:text-orange-600 transition-colors"
            >
              Khám phá công thức
            </Link>
            <a
              href="#categories"
              onClick={() => setIsMobileMenuOpen(false)}
              className="px-2 py-1.5 rounded-md hover:bg-orange-50 hover:text-orange-600 transition-colors"
            >
              Danh mục món ăn
            </a>
            <a
              href="#about"
              onClick={() => setIsMobileMenuOpen(false)}
              className="px-2 py-1.5 rounded-md hover:bg-orange-50 hover:text-orange-600 transition-colors"
            >
              Về chúng tôi
            </a>
          </nav>

          <div className="pt-3 border-t border-neutral-100 flex flex-col gap-2">
            <Link
              href="/recipes/create"
              onClick={() => setIsMobileMenuOpen(false)}
              className="flex items-center justify-center gap-2 rounded-lg bg-orange-50 py-2.5 text-sm font-semibold text-orange-700"
            >
              <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
              </svg>
              Viết công thức mới
            </Link>
            <div className="grid grid-cols-2 gap-2 mt-1">
              <Link
                href="/login"
                onClick={() => setIsMobileMenuOpen(false)}
                className="flex items-center justify-center rounded-lg border border-neutral-200 py-2 text-sm font-medium text-neutral-700 hover:bg-neutral-50"
              >
                Đăng nhập
              </Link>
              <Link
                href="/register"
                onClick={() => setIsMobileMenuOpen(false)}
                className="flex items-center justify-center rounded-lg bg-orange-600 py-2 text-sm font-semibold text-white hover:bg-orange-700"
              >
                Đăng ký
              </Link>
            </div>
          </div>
        </div>
      )}
    </header>
  );
}
