<script setup></script>

<template>
  <div class="flex flex-col">
    <div class="overflow-x-auto sm:mx-0.5 lg:mx-0.5">
      <div class="inline-block min-w-full py-2 sm:px-6 lg:px-8">
        <div class="overflow-hidden">
          <table class="min-w-full">
            <thead class="border-b bg-white">
              <tr>
                <th scope="col" class="px-6 py-4 text-left text-sm font-medium text-gray-900">#</th>
                <th scope="col" class="px-6 py-4 text-left text-sm font-medium text-gray-900">
                  First
                </th>
                <th scope="col" class="px-6 py-4 text-left text-sm font-medium text-gray-900">
                  Last
                </th>
                <th scope="col" class="px-6 py-4 text-left text-sm font-medium text-gray-900">
                  Handle
                </th>
              </tr>
            </thead>
            <tbody>
              <tr class="border-b bg-gray-100">
                <td class="px-6 py-4 text-sm font-medium whitespace-nowrap text-gray-900">1</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">Mark</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">Otto</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">@mdo</td>
              </tr>
              <tr class="border-b bg-white">
                <td class="px-6 py-4 text-sm font-medium whitespace-nowrap text-gray-900">2</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">Jacob</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">Dillan</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">@fat</td>
              </tr>
              <tr class="border-b bg-gray-100">
                <td class="px-6 py-4 text-sm font-medium whitespace-nowrap text-gray-900">3</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">Mark</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">Twen</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">
                  @twitter
                </td>
              </tr>
              <tr class="border-b bg-white">
                <td class="px-6 py-4 text-sm font-medium whitespace-nowrap text-gray-900">4</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">Bob</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">Dillan</td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">@fat</td>
              </tr>
              <tr class="border-b bg-gray-100">
                <td class="px-6 py-4 text-sm font-medium whitespace-nowrap text-gray-900">5</td>
                <td
                  colspan="2"
                  class="px-6 py-4 text-center text-sm font-light whitespace-nowrap text-gray-900"
                >
                  Larry the Bird
                </td>
                <td class="px-6 py-4 text-sm font-light whitespace-nowrap text-gray-900">
                  @twitter
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
