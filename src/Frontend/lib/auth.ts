// Cấu hình Auth.js v5. Access token của backend được nhúng vào session JWT
// của Auth.js (lưu trong cookie HTTP-only), không lưu localStorage để tránh XSS.
import NextAuth, { type DefaultSession } from "next-auth";
import Credentials from "next-auth/providers/credentials";
import Google from "next-auth/providers/google";

declare module "next-auth" {
  interface Session {
    user: { id: string; accessToken: string } & DefaultSession["user"];
  }
  interface User {
    accessToken?: string;
    refreshToken?: string;
  }
}

export const { handlers, auth, signIn, signOut } = NextAuth({
  providers: [
    Credentials({
      credentials: {
        email: { label: "Email", type: "email" },
        password: { label: "Mật khẩu", type: "password" },
      },
      async authorize(credentials) {
        if (!credentials?.email || !credentials?.password) return null;

        try {
          const response = await fetch(`${process.env.API_URL}/api/v1/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
              email: credentials.email,
              password: credentials.password,
            }),
          });

          if (!response.ok) return null;

          // Backend bọc response trong { data } (SRS mục 8).
          const body = await response.json();
          const tokens = body.data;

          // TODO (người phụ trách FR-AUTH): endpoint /auth/login chỉ trả token,
          // nên cần gọi thêm GET /auth/me để lấy displayName/avatar rồi map vào
          // đây cho đủ thông tin hiển thị.
          return {
            id: credentials.email as string,
            email: credentials.email as string,
            accessToken: tokens.accessToken,
            refreshToken: tokens.refreshToken,
          };
        } catch {
          return null;
        }
      },
    }),

    // FR-AUTH-003. Sau khi Google trả về, cần gọi POST /api/v1/auth/google
    // với idToken để đổi lấy JWT của hệ thống mình.
    Google({
      clientId: process.env.GOOGLE_CLIENT_ID ?? "",
      clientSecret: process.env.GOOGLE_CLIENT_SECRET ?? "",
    }),
  ],

  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.accessToken = user.accessToken;
        token.refreshToken = user.refreshToken;
        token.userId = user.id;
      }
      // TODO (người phụ trách FR-AUTH-004): access token chỉ sống 15 phút,
      // cần bổ sung logic tự gọi /auth/refresh khi token sắp hết hạn.
      return token;
    },

    async session({ session, token }) {
      session.user.id = token.userId as string;
      session.user.accessToken = token.accessToken as string;
      return session;
    },
  },

  pages: { signIn: "/login" },
});
