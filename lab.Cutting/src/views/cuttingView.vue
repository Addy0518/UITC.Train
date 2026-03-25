<script setup>
import { ref } from 'vue'
// 卡片數量
const cardcount = ref(9)
</script>

<template>
  <!-- 導覽列 -->
  <div class="bg-black w-full">
    <div class="container max-w-screen-xl flex items-center mx-auto px-4">
      <div class="mt-5 mb-5">
        <a href="#" class="navbar-brand d-flex align-items-center">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            fill="none"
            stroke="currentColor"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            aria-hidden="true"
            class="me-2 text-white"
            viewBox="0 0 24 24"
          >
            <path
              d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z"
            ></path>
            <circle cx="12" cy="13" r="4"></circle>
          </svg>
        </a>
      </div>

      <div>
        <strong class="text-white text-2xl">Album</strong>
      </div>
      <div class="ml-auto">
        <div class="border-1 text-white m-1 px-3 rounded-xl text-5xl opacity-50">≡</div>
      </div>
    </div>
  </div>
  <!-- 刊頭 -->
  <div class="container max-w-screen-xl flex items-center mx-auto py-28">
    <article class="text-center max-w-2xl mx-auto">
      <p class="text-center text-5xl mb-4 font-light">Album example</p>
      <p class="text-center text-lg mb-4 opacity-60 font-light">
        Something short and leading about the collection below—its contents, the creator, etc. Make
        it short and sweet, but not too short so folks don’t simply skip over it entirely.
      </p>
      <div class="flex justify-center gap-2 text-lg font-light">
        <p class="bg-blue-600 text-white px-4 py-2 rounded-md">Main call to action</p>
        <p class="bg-gray-500 text-white px-4 py-2 rounded-md">Secondary action</p>
      </div>
    </article>
  </div>
  <!-- 卡片總區域 -->
  <div class="bg-gray-100">
    <div class="container max-w-screen-xl mx-auto px-4 pt-5 pb-5">
      <div class="grid grid-cols-3 gap-6 my-10">
        <!-- 卡片 -->
        <div v-for="card in cardcount" :key="card">
          <div class="bg-white shadow-lg/10 rounded-md overflow-hidden relative h-fit">
            <div class="bg-black opacity-60 aspect-video flex items-center justify-center">
              <div class="flex text-white justify-center-safe text-xl">Thumbnail</div>
            </div>
            <div class="p-4 wrap-anywhere">
              <article class="flex justify-left-safe text-wrap">
                <p class="text-pretty text-base mb-3">
                  This is a wider card with supporting text below as a natural lead-in to additional
                  content. This content is a little bit longer.
                </p>
              </article>
              <div class="flex justify-between items-center-safe">
                <div class="inline-flex opacity-70">
                  <button class="border p-1 rounded-l-md px-2 py-1">View</button>
                  <button class="border border-l-0 p-1 rounded-r-md px-2 py-1">Edit</button>
                </div>
                <div class="p-1 opacity-75 items-end-safe">9 mins</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <!-- 頁尾 -->
  <div class="w-full">
    <div class="container max-w-screen-xl flex items-center mx-auto pb-10 pt-10">
      <div class="">
        <p>Album example is © Bootstrap, but please download and customize it for yourself!</p>
        <p>
          New to Bootstrap?
          <a href="" class="underline text-blue-500">Visit the homepage</a> or read our
          <a href="" class="underline text-blue-500">getting started guide.</a>
        </p>
      </div>
      <div class="ml-auto">
        <a href="" class="underline text-blue-500">Back to top</a>
      </div>
    </div>
  </div>
</template>

<style></style>
