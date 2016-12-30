#define __NV_CUBIN_HANDLE_STORAGE__ static
#include "crt/host_runtime.h"
#include "kernel.fatbin.c"
extern void __device_stub__Z6kerneliiPi(int, int, int *);
static void __nv_cudaEntityRegisterCallback(void **);
static void __sti____cudaRegisterAll_14_kernel_cpp1_ii_b614c5e6(void);
#pragma section(".CRT$XCU",read,write)
__declspec(allocate(".CRT$XCU"))static void (*__dummy_static_init__sti____cudaRegisterAll_14_kernel_cpp1_ii_b614c5e6[])(void) = {__sti____cudaRegisterAll_14_kernel_cpp1_ii_b614c5e6};
void __device_stub__Z6kerneliiPi(int __par0, int __par1, int *__par2){__cudaSetupArgSimple(__par0, 0U);__cudaSetupArgSimple(__par1, 4U);__cudaSetupArgSimple(__par2, 8U);__cudaLaunch(((char *)((void ( *)(int, int, int *))kernel)));}
#line 4 "C:/Users/w0ns88/Desktop/pba/pa/p-exam/MatrixMultiplication/CUDA/kernel.cu"
void kernel( int __cuda_0,int __cuda_1,int *__cuda_2)
#line 5 "C:/Users/w0ns88/Desktop/pba/pa/p-exam/MatrixMultiplication/CUDA/kernel.cu"
{__device_stub__Z6kerneliiPi( __cuda_0,__cuda_1,__cuda_2);

}
#line 1 "Debug/kernel.cudafe1.stub.c"
static void __nv_cudaEntityRegisterCallback( void **__T20) {  __nv_dummy_param_ref(__T20); __nv_save_fatbinhandle_for_managed_rt(__T20); __cudaRegisterEntry(__T20, ((void ( *)(int, int, int *))kernel), _Z6kerneliiPi, (-1)); }
static void __sti____cudaRegisterAll_14_kernel_cpp1_ii_b614c5e6(void) {  __cudaRegisterBinary(__nv_cudaEntityRegisterCallback);  }
